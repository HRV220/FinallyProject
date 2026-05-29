using Transactions.Domain.Enums;

namespace Transactions.Application.Transactions.Queries.GetTransactionsByWalletId;

public record GetTransactionsByWalletIdResponse(
  Guid Id,
  Guid WalletId,
  Guid? CategoryId,
  FinancialType Type,
  decimal Amount,
  DateOnly Date,
  string? Description);
