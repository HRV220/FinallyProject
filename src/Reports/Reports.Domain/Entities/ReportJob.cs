using Reports.Domain.Enums;
using Core.Domain.Entities;
using Core.Domain.Common;

namespace Reports.Domain.Entities;
public class ReportJob : BaseEntity
{
  public ReportStatus Status { get; private set; }
  public ReportType Type { get; private set; }
  public DateTime? CompletedAt { get; private set; }
  public string? FileKey { get; private set; }
  public string? Error { get; private set; }
  public Guid RequestedBy { get; private set; }
  public string ParametersJson { get; private set; } = "{}";

  private ReportJob() : base()
  {

  }

  private ReportJob(Guid id, ReportType type, Guid requestedBy, string parametersJson) : base(id)
  {
    Type = type;
    Status = ReportStatus.Pending;
    RequestedBy = requestedBy;
    ParametersJson = parametersJson;
  }

  public static Result<ReportJob> Create(ReportType type, Guid requestedBy, string parametersJson)
  {
    if (requestedBy == Guid.Empty)
      return Result<ReportJob>.Failure(new DomainError("ReportJob.InvalidRequestedBy", "RequestedBy is required."));

    if (string.IsNullOrWhiteSpace(parametersJson))
      parametersJson = "{}";

    return Result<ReportJob>.Success(new ReportJob(Guid.NewGuid(), type, requestedBy, parametersJson));
  }
  public Result<bool> MarkProcessing()
  {
    if (Status != ReportStatus.Pending)
      return Result<bool>.Failure(new DomainError("ReportJob.InvalidTransition",
        $"Can only start processing from Pending state, current: {Status}."));

    Status = ReportStatus.Processing;
    SetUpdated();
    return Result<bool>.Success(true);
  }
  public Result<bool> MarkCompleted(string fileKey)
  {
    if (Status != ReportStatus.Processing)
      return Result<bool>.Failure(new DomainError("ReportJob.InvalidTransition",
        $"Can only complete from Processing state, current: {Status}."));

    if (string.IsNullOrWhiteSpace(fileKey))
      return Result<bool>.Failure(new DomainError("ReportJob.InvalidFileKey", "FileKey is required."));

    Status = ReportStatus.Completed;
    FileKey = fileKey;
    CompletedAt = DateTime.UtcNow;
    SetUpdated();
    return Result<bool>.Success(true);
  }
  public Result<bool> MarkFailed(string error)
  {
    if (Status == ReportStatus.Completed)
      return Result<bool>.Failure(new DomainError("ReportJob.InvalidTransition",
        "Cannot fail an already completed job."));

    if (string.IsNullOrWhiteSpace(error))
      error = "Unknown error";

    if (error.Length > 2000)
      error = error.Substring(0, 2000);

    Status = ReportStatus.Failed;
    Error = error;
    CompletedAt = DateTime.UtcNow;
    SetUpdated();
    return Result<bool>.Success(true);
  }


}