using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Wallets.Commands.ChangeIcon;

public class ChangeIconCommandHandler : IRequestHandler<ChangeIconCommand, Result<bool>>
{
  private readonly IWalletRepository _walletRepository;

  public ChangeIconCommandHandler(IWalletRepository walletRepository)
  {
    _walletRepository = walletRepository;
  }

  public async Task<Result<bool>> Handle(ChangeIconCommand command, CancellationToken cancellationToken)
  {
    var wallet = await _walletRepository.GetWalletByIdAsync(command.Id);
    if (wallet is null)
      return Result<bool>.Failure(new DomainError("Wallet.NotFound", "Wallet not found."));

    wallet.ChangeIcon(command.NewIcon);
    await _walletRepository.UpdateAsync(wallet);
    return Result<bool>.Success(true);
  }
}
