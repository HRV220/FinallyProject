using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Enums;

namespace Transactions.Application.Analytics.Queries.GetCategoryAnalytics;

public record GetCategoryAnalyticsQuery(
    Guid ProfileId,
    FinancialType Type,
    DateOnly? DateFrom,
    DateOnly? DateTo) : IRequest<Result<IEnumerable<GetCategoryAnalyticsResponse>>>;
