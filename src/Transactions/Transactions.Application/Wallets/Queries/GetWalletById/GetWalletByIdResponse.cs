namespace Transactions.Application.Wallets.Queries.GetWalletById;

public record GetWalletByIdResponse(
  Guid Id,
  Guid ProfileId,
  string Name,
  string? Icon,
  int SortOrder,
  Guid CurrencyId,
  decimal InitialBalance,
  string? Note,
  bool IsArchived);
