namespace Reports.Application.Reports.Commands.CreateReport;

public record CreateProfileTransactionsReportRequest(
  Guid ProfileId,
  DateOnly From,
  DateOnly To);
