using Reports.Domain.Enums;

namespace Reports.Application.Reports.Queries.GetReportStatus;

public record GetReportStatusResponse(
  Guid Id,
  ReportType Type,
  ReportStatus Status,
  DateTime CreatedAt,
  DateTime? CompletedAt,
  string? Error);