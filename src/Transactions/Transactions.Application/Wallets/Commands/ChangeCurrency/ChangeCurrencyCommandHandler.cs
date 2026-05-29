using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Wallets.Commands.ChangeCurrency;

public class ChangeCurrencyCommandHandler : IRequestHandler<ChangeCurrencyCommand, Result<bool>>
{
  private readonly IWalletRepository _walletRepository;
  private readonly ICurrencyRepository _currencyRepository;

  public ChangeCurrencyCommandHandler(IWalletRepository walletRepository, ICurrencyRepository currencyRepository)
  {
    _walletRepository = walletRepository;
    _currencyRepository = currencyRepository;
  }

  public async Task<Result<bool>> Handle(ChangeCurrencyCommand command, CancellationToken cancellationToken)
  {
    var wallet = await _walletRepository.GetWalletByIdAsync(command.Id);
    if (wallet is null)
      return Result<bool>.Failure(new DomainError("Wallet.NotFound", "Wallet not found."));

    var currency = await _currencyRepository.GetByIdAsync(command.NewCurrencyId);
    if (currency is null)
      return Result<bool>.Failure(new DomainError("Wallet.CurrencyNotFound", "Currency not found."));

    wallet.ChangeCurrency(command.NewCurrencyId);
    await _walletRepository.UpdateAsync(wallet);
    return Result<bool>.Success(true);
  }
}
