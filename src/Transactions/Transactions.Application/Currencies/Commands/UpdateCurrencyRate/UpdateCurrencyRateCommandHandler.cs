using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Currencies.Commands.UpdateCurrencyRate;

public class UpdateCurrencyRateCommandHandler : IRequestHandler<UpdateCurrencyRateCommand, Result<bool>>
{
  private readonly ICurrencyRepository _currencyRepository;

  public UpdateCurrencyRateCommandHandler(ICurrencyRepository currencyRepository)
  {
    _currencyRepository = currencyRepository;
  }

  public async Task<Result<bool>> Handle(UpdateCurrencyRateCommand command, CancellationToken cancellationToken)
  {
    var currency = await _currencyRepository.GetByIdAsync(command.Id);
    if (currency is null)
      return Result<bool>.Failure(new DomainError("Currency.NotFound", "Currency not found."));

    currency.UpdateRate(command.Rate, command.UnitRate);
    await _currencyRepository.UpdateAsync(currency);
    return Result<bool>.Success(true);
  }
}
