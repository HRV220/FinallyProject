using Transactions.Domain.Enums;

namespace Transactions.Application.Transactions.Queries.GetTransactionById;

public record GetTransactionByIdResponse(
  Guid Id,
  Guid WalletId,
  Guid? CategoryId,
  FinancialType Type,
  decimal Amount,
  DateOnly Date,
  string? Description);
