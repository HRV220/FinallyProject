using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Wallets.Commands.ChangeSortOrder;

public class ChangeSortOrderCommandHandler : IRequestHandler<ChangeSortOrderCommand, Result<bool>>
{
  private readonly IWalletRepository _walletRepository;

  public ChangeSortOrderCommandHandler(IWalletRepository walletRepository)
  {
    _walletRepository = walletRepository;
  }

  public async Task<Result<bool>> Handle(ChangeSortOrderCommand command, CancellationToken cancellationToken)
  {
    var wallet = await _walletRepository.GetWalletByIdAsync(command.Id);
    if (wallet is null)
      return Result<bool>.Failure(new DomainError("Wallet.NotFound", "Wallet not found."));

    var result = wallet.ChangeSortOrder(command.NewSortOrder);
    if (result.IsFailure)
      return Result<bool>.Failure(result.Error!);

    await _walletRepository.UpdateAsync(wallet);
    return Result<bool>.Success(true);
  }
}
