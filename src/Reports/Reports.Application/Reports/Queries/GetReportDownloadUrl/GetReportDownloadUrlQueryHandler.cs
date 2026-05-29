using Core.Domain.Common;
using MediatR;
using Reports.Application.Abstractions;
using Reports.Domain.Enums;
using Reports.Domain.Interfaces;

namespace Reports.Application.Reports.Queries.GetReportDownloadUrl;

public class GetReportDownloadUrlQueryHandler : IRequestHandler<GetReportDownloadUrlQuery, Result<GetReportDownloadUrlResponse>>
{
  private static readonly TimeSpan UrlValidity = TimeSpan.FromMinutes(15);

  private readonly IReportJobRepository _jobRepository;
  private readonly IFileStorage _fileStorage;

  public GetReportDownloadUrlQueryHandler(IReportJobRepository jobRepository, IFileStorage fileStorage)
  {
    _jobRepository = jobRepository;
    _fileStorage = fileStorage;
  }

  public async Task<Result<GetReportDownloadUrlResponse>> Handle(GetReportDownloadUrlQuery query, CancellationToken cancellationToken = default)
  {
    var job = await _jobRepository.GetByIdAsync(query.ReportId, cancellationToken);
    if (job is null)
      return Result<GetReportDownloadUrlResponse>.Failure(
        new DomainError("Report.NotFound", "Report not found."));

    if (job.RequestedBy != query.RequestedBy)
      return Result<GetReportDownloadUrlResponse>.Failure(
        new DomainError("Report.NotFound", "Report not found."));

    if (job.Status != ReportStatus.Completed)
      return Result<GetReportDownloadUrlResponse>.Failure(
        new DomainError("Report.NotReady", $"Report is not ready. Current status: {job.Status}."));

    if (string.IsNullOrWhiteSpace(job.FileKey))
      return Result<GetReportDownloadUrlResponse>.Failure(
        new DomainError("Report.NotReady", "Report has no file."));

    var url = _fileStorage.BuildDownloadUrl(job.FileKey);
    return Result<GetReportDownloadUrlResponse>.Success(new GetReportDownloadUrlResponse(url, DateTime.UtcNow.Add(UrlValidity)));
  }
}
