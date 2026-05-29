using Core.Infrastructure;
using Core.Infrastructure.Correlation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;
using Reports.Application.Abstractions;
using Reports.Domain.Interfaces;
using Reports.Infrastructure.BackgroundWorkers;
using Reports.Infrastructure.Clients;
using Reports.Infrastructure.Generation;
using Reports.Infrastructure.Persistence;
using Reports.Infrastructure.Persistence.Repositories;
using Reports.Infrastructure.Storage;

namespace Reports.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddReportsModule(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<ReportsDbContext>(options =>
      options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", ReportsDbContext.Schema)));

    services.AddScoped<IReportJobRepository, ReportJobRepository>();
    services.AddScoped<IFileStorage, LocalFileStorage>();
    services.AddScoped<IReportGenerator, CsvReportGenerator>();

    services.AddPlatformHttpEssentials();

    AddResilientRefitClient<IUsersServiceClient>(services,
      configuration["UsersService:BaseUrl"]
        ?? throw new InvalidOperationException("UsersService:BaseUrl is required."));

    AddResilientRefitClient<ICategoriesServiceClient>(services,
      configuration["CategoriesService:BaseUrl"]
        ?? throw new InvalidOperationException("CategoriesService:BaseUrl is required."));

    AddResilientRefitClient<IAnalyticsServiceClient>(services,
      configuration["TransactionsService:BaseUrl"]
        ?? throw new InvalidOperationException("TransactionsService:BaseUrl is required."));

    services.AddHostedService<ReportProcessingWorker>();

    return services;
  }

  private static void AddResilientRefitClient<TClient>(IServiceCollection services, string baseUrl) where TClient : class
  {
    services
      .AddRefitClient<TClient>()
      .ConfigureHttpClient(c =>
      {
        c.BaseAddress = new Uri(baseUrl);
        c.Timeout = TimeSpan.FromSeconds(15);
      })
      .AddHttpMessageHandler<CorrelationIdHandler>()
      .AddStandardResilienceHandler(options =>
      {
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
        options.Retry.MaxRetryAttempts = 2;
        options.Retry.Delay = TimeSpan.FromMilliseconds(300);
        options.Retry.BackoffType = DelayBackoffType.Exponential;
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(15);
      });
  }
}
