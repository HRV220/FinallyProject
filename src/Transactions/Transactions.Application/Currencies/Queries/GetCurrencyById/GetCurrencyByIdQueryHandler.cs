using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Currencies.Queries.GetCurrencyById;

public class GetCurrencyByIdQueryHandler : IRequestHandler<GetCurrencyByIdQuery, Result<GetCurrencyByIdResponse>>
{
  private readonly ICurrencyRepository _currencyRepository;

  public GetCurrencyByIdQueryHandler(ICurrencyRepository currencyRepository)
  {
    _currencyRepository = currencyRepository;
  }

  public async Task<Result<GetCurrencyByIdResponse>> Handle(GetCurrencyByIdQuery query, CancellationToken cancellationToken)
  {
    var currency = await _currencyRepository.GetByIdAsync(query.Id);
    if (currency is null)
      return Result<GetCurrencyByIdResponse>.Failure(new DomainError("Currency.NotFound", "Currency not found."));

    return Result<GetCurrencyByIdResponse>.Success(new GetCurrencyByIdResponse(
      currency.Id,
      currency.Name,
      currency.Code,
      currency.NumericCode,
      currency.Nominal,
      currency.Rate,
      currency.UnitRate));
  }
}
