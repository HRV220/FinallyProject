using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Currencies.Queries.GetAllCurrencies;

public class GetAllCurrenciesQueryHandler : IRequestHandler<GetAllCurrenciesQuery, Result<IEnumerable<GetAllCurrenciesResponse>>>
{
  private readonly ICurrencyRepository _currencyRepository;

  public GetAllCurrenciesQueryHandler(ICurrencyRepository currencyRepository)
  {
    _currencyRepository = currencyRepository;
  }

  public async Task<Result<IEnumerable<GetAllCurrenciesResponse>>> Handle(GetAllCurrenciesQuery query, CancellationToken cancellationToken)
  {
    var currencies = await _currencyRepository.GetAllAsync();

    var response = currencies.Select(c => new GetAllCurrenciesResponse(
      c.Id,
      c.Name,
      c.Code,
      c.NumericCode,
      c.Nominal,
      c.Rate,
      c.UnitRate));

    return Result<IEnumerable<GetAllCurrenciesResponse>>.Success(response);
  }
}
