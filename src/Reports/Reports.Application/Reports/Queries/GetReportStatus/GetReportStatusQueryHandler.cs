using MediatR;
using Core.Domain.Common;
using Reports.Domain.Interfaces;

namespace Reports.Application.Reports.Queries.GetReportStatus;

public class GetReportStatusQueryHandler : IRequestHandler<GetReportStatusQuery, Result<GetReportStatusResponse>>
{
  private readonly IReportJobRepository _jobRepository;

  public GetReportStatusQueryHandler(IReportJobRepository jobRepository)
  {
    _jobRepository = jobRepository;
  }

  public async Task<Result<GetReportStatusResponse>> Handle(GetReportStatusQuery query, CancellationToken cancellationToken = default)
  {
    var job = await _jobRepository.GetByIdAsync(query.ReportId, cancellationToken);
    if (job is null)
      return Result<GetReportStatusResponse>.Failure(
        new DomainError("Report.NotFound", "Report not found."));

    if (job.RequestedBy != query.RequestedBy)
      return Result<GetReportStatusResponse>.Failure(
        new DomainError("Report.AccessDenied", "Report not found."));

    return Result<GetReportStatusResponse>.Success(new GetReportStatusResponse(
      job.Id,
      job.Type,
      job.Status,
      job.CreatedAt,
      job.CompletedAt,
      job.Error));
  }
}