using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Wallets.Commands.DeleteWallet;

public class DeleteWalletCommandHandler : IRequestHandler<DeleteWalletCommand, Result<bool>>
{
  private readonly IWalletRepository _walletRepository;

  public DeleteWalletCommandHandler(IWalletRepository walletRepository)
  {
    _walletRepository = walletRepository;
  }

  public async Task<Result<bool>> Handle(DeleteWalletCommand command, CancellationToken cancellationToken)
  {
    var wallet = await _walletRepository.GetWalletByIdAsync(command.Id);
    if (wallet is null)
      return Result<bool>.Failure(new DomainError("Wallet.NotFound", "Wallet not found."));

    await _walletRepository.DeleteAsync(command.Id);
    return Result<bool>.Success(true);
  }
}
