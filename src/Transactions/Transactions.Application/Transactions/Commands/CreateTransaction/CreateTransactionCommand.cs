using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Enums;

namespace Transactions.Application.Transactions.Commands.CreateTransaction;

public record CreateTransactionCommand(
  Guid WalletId,
  FinancialType Type,
  decimal Amount,
  DateOnly Date,
  Guid? CategoryId = null,
  string? Description = null,
  Guid? ToWalletId = null) : IRequest<Result<Guid>>;
