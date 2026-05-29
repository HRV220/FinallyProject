using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Wallets.Commands.ChangeNote;

public class ChangeNoteCommandHandler : IRequestHandler<ChangeNoteCommand, Result<bool>>
{
  private readonly IWalletRepository _walletRepository;

  public ChangeNoteCommandHandler(IWalletRepository walletRepository)
  {
    _walletRepository = walletRepository;
  }

  public async Task<Result<bool>> Handle(ChangeNoteCommand command, CancellationToken cancellationToken)
  {
    var wallet = await _walletRepository.GetWalletByIdAsync(command.Id);
    if (wallet is null)
      return Result<bool>.Failure(new DomainError("Wallet.NotFound", "Wallet not found."));

    wallet.ChangeNote(command.NewNote);
    await _walletRepository.UpdateAsync(wallet);
    return Result<bool>.Success(true);
  }
}
