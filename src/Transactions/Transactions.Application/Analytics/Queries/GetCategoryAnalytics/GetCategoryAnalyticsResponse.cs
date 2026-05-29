namespace Transactions.Application.Analytics.Queries.GetCategoryAnalytics;

public record GetCategoryAnalyticsResponse(
    Guid? CategoryId,
    decimal Value,
    string Color);
