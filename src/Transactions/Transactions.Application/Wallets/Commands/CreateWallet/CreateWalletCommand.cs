using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Wallets.Commands.CreateWallet;

public record CreateWalletCommand(
  Guid ProfileId,
  string Name,
  int SortOrder,
  Guid CurrencyId,
  decimal InitialBalance,
  string? Icon = null,
  string? Note = null) : IRequest<Result<Guid>>;
