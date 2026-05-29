using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Currencies.Queries.GetAllCurrencies;

public record GetAllCurrenciesQuery : IRequest<Result<IEnumerable<GetAllCurrenciesResponse>>>;
