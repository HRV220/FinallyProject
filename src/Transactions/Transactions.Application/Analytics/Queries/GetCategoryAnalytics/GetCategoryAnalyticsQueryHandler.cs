using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Analytics.Queries.GetCategoryAnalytics;

public class GetCategoryAnalyticsQueryHandler : IRequestHandler<GetCategoryAnalyticsQuery, Result<IEnumerable<GetCategoryAnalyticsResponse>>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetCategoryAnalyticsQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Result<IEnumerable<GetCategoryAnalyticsResponse>>> Handle(GetCategoryAnalyticsQuery query, CancellationToken cancellationToken)
    {
        var totals = await _transactionRepository.GetCategoryTotalsAsync(
            query.ProfileId, query.Type, query.DateFrom, query.DateTo);

        var response = totals
            .OrderByDescending(t => t.Total)
            .Select(t => new GetCategoryAnalyticsResponse(
                t.CategoryId,
                t.Total,
                IdToColor(t.CategoryId)))
            .ToList();

        return Result<IEnumerable<GetCategoryAnalyticsResponse>>.Success(response);
    }

    // Стабильный цвет по CategoryId. Имена категорий клиент резолвит через Categories Service.
    private static string IdToColor(Guid? id)
    {
        if (id is null) return "hsl(0, 0%, 60%)";
        int hue = ((id.Value.GetHashCode() % 360) + 360) % 360;
        return $"hsl({hue}, 65%, 55%)";
    }
}
