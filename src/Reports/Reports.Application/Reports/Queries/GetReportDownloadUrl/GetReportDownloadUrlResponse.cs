namespace Reports.Application.Reports.Queries.GetReportDownloadUrl;

public record GetReportDownloadUrlResponse(string DownloadUrl, DateTime ExpiresAt);