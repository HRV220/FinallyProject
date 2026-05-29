using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Currencies.Queries.GetCurrencyById;

public record GetCurrencyByIdQuery(Guid Id) : IRequest<Result<GetCurrencyByIdResponse>>;
