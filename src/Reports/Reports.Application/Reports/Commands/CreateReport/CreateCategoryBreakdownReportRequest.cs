namespace Reports.Application.Reports.Commands.CreateReport;

public record CreateCategoryBreakdownReportRequest(
  Guid ProfileId,
  DateOnly From,
  DateOnly To);
