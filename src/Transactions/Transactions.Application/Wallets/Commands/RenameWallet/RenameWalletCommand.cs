using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Wallets.Commands.RenameWallet;

public record RenameWalletCommand(Guid Id, string NewName) : IRequest<Result<bool>>;
