using Core.Domain.Common;
using MediatR;
using Reports.Domain.Entities;
using Reports.Domain.Interfaces;

namespace Reports.Application.Reports.Commands.CreateReport;

public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Result<CreateReportResponse>>
{
  private readonly IReportJobRepository _jobRepository;

  public CreateReportCommandHandler(IReportJobRepository jobRepository)
  {
    _jobRepository = jobRepository;
  }

  public async Task<Result<CreateReportResponse>> Handle(CreateReportCommand command, CancellationToken cancellationToken = default)
  {
    var jobResult = ReportJob.Create(command.Type, command.RequestedBy, command.ParametersJson);
    if (jobResult.IsFailure)
      return Result<CreateReportResponse>.Failure(jobResult.Error!);

    var job = jobResult.Value!;
    await _jobRepository.CreateAsync(job, cancellationToken);

    // Stage 3: job создаётся в статусе Pending. Реальная генерация PDF
    // (background pipeline, S3, HTTP-вызовы к Transactions/Categories) — на Этапе 4.
    return Result<CreateReportResponse>.Success(new CreateReportResponse(job.Id));
  }
}
