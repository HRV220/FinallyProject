namespace Transactions.Application.Transactions.Queries.GetWalletBalance;

public record GetWalletBalanceResponse(Guid WalletId, decimal Balance);
