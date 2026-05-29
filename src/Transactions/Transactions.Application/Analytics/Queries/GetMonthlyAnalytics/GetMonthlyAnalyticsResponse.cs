namespace Transactions.Application.Analytics.Queries.GetMonthlyAnalytics;

public record GetMonthlyAnalyticsResponse(
    string Month,
    decimal Income,
    decimal Expense,
    decimal Balance);
