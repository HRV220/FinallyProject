using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Analytics.Queries.GetMonthlyAnalytics;

public record GetMonthlyAnalyticsQuery(Guid ProfileId, int Year) : IRequest<Result<IEnumerable<GetMonthlyAnalyticsResponse>>>;
