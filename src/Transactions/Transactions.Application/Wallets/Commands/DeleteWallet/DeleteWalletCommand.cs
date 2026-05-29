using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Wallets.Commands.DeleteWallet;

public record DeleteWalletCommand(Guid Id) : IRequest<Result<bool>>;
