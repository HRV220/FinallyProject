using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Analytics.Queries.GetMonthlyAnalytics;

public class GetMonthlyAnalyticsQueryHandler : IRequestHandler<GetMonthlyAnalyticsQuery, Result<IEnumerable<GetMonthlyAnalyticsResponse>>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetMonthlyAnalyticsQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<IEnumerable<GetMonthlyAnalyticsResponse>>> Handle(GetMonthlyAnalyticsQuery query, CancellationToken cancellationToken)
    {
        var totals = await _transactionRepository.GetMonthlyTotalsAsync(query.ProfileId, query.Year);
        var byMonth = totals.ToDictionary(t => t.Month);

        var result = new List<GetMonthlyAnalyticsResponse>(12);
        decimal cumulativeBalance = 0;

        for (int m = 1; m <= 12; m++)
        {
            var income = byMonth.TryGetValue(m, out var row) ? row.Income : 0m;
            var expense = byMonth.TryGetValue(m, out row) ? row.Expense : 0m;

            cumulativeBalance += income - expense;

            result.Add(new GetMonthlyAnalyticsResponse(
                $"{query.Year:D4}-{m:D2}",
                income,
                expense,
                cumulativeBalance));
        }

        return Result<IEnumerable<GetMonthlyAnalyticsResponse>>.Success(result);
    }
}
