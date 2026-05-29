using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Reports.Application.Abstractions;
using Reports.Domain.Entities;
using Reports.Domain.Interfaces;

namespace Reports.Infrastructure.BackgroundWorkers;

/// <summary>
/// Stage 4 (минимально): пул-цикл, который раз в N секунд берёт Pending-задачи и обрабатывает их.
/// В реальном проде заменяется на брокер (RabbitMQ/Hangfire), интерфейсы IFileStorage/IReportGenerator не меняются.
/// </summary>
public class ReportProcessingWorker : BackgroundService
{
  private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(5);
  private const int BatchSize = 5;

  private readonly IServiceScopeFactory _scopeFactory;
  private readonly ILogger<ReportProcessingWorker> _logger;

  public ReportProcessingWorker(IServiceScopeFactory scopeFactory, ILogger<ReportProcessingWorker> logger)
  {
    _scopeFactory = scopeFactory;
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("ReportProcessingWorker started.");

    while (!stoppingToken.IsCancellationRequested)
    {
      try
      {
        await ProcessBatchAsync(stoppingToken);
      }
      catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
      {
        break;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Unexpected error in ReportProcessingWorker loop.");
      }

      try
      {
        await Task.Delay(PollInterval, stoppingToken);
      }
      catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
      {
        break;
      }
    }

    _logger.LogInformation("ReportProcessingWorker stopped.");
  }

  private async Task ProcessBatchAsync(CancellationToken ct)
  {
    using var scope = _scopeFactory.CreateScope();
    var jobs = scope.ServiceProvider.GetRequiredService<IReportJobRepository>();
    var pending = await jobs.GetPendingAsync(BatchSize, ct);
    if (pending.Count == 0) return;

    foreach (var job in pending)
      await ProcessOneAsync(job, ct);
  }

  private async Task ProcessOneAsync(ReportJob job, CancellationToken ct)
  {
    using var scope = _scopeFactory.CreateScope();
    var jobs = scope.ServiceProvider.GetRequiredService<IReportJobRepository>();
    var generator = scope.ServiceProvider.GetRequiredService<IReportGenerator>();
    var storage = scope.ServiceProvider.GetRequiredService<IFileStorage>();

    var transition = job.MarkProcessing();
    if (transition.IsFailure)
    {
      _logger.LogWarning("Skip job {JobId}: {Code} {Message}", job.Id, transition.Error!.Code, transition.Error.Message);
      return;
    }
    await jobs.UpdateAsync(job, ct);

    try
    {
      var report = await generator.GenerateAsync(job, ct);
      var key = await storage.SaveAsync(report.FileKey, report.Content, report.ContentType, ct);

      var completed = job.MarkCompleted(key);
      if (completed.IsFailure)
        throw new InvalidOperationException(completed.Error!.Message);

      await jobs.UpdateAsync(job, ct);
      _logger.LogInformation("Report {JobId} completed -> {Key}", job.Id, key);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Report {JobId} failed.", job.Id);
      job.MarkFailed(ex.Message);
      try { await jobs.UpdateAsync(job, ct); }
      catch (Exception updateEx) { _logger.LogError(updateEx, "Failed to mark report {JobId} as failed.", job.Id); }
    }
  }
}
