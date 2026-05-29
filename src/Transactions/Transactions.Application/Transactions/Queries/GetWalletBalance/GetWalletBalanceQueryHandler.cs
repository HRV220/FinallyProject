using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Enums;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Transactions.Queries.GetWalletBalance;

public class GetWalletBalanceQueryHandler : IRequestHandler<GetWalletBalanceQuery, Result<GetWalletBalanceResponse>>
{
  private readonly IWalletRepository _walletRepository;
  private readonly ITransactionRepository _transactionRepository;

  public GetWalletBalanceQueryHandler(IWalletRepository walletRepository, ITransactionRepository transactionRepository)
  {
    _walletRepository = walletRepository;
    _transactionRepository = transactionRepository;
  }

  public async Task<Result<GetWalletBalanceResponse>> Handle(GetWalletBalanceQuery query, CancellationToken cancellationToken)
  {
    var wallet = await _walletRepository.GetWalletByIdAsync(query.WalletId);
    if (wallet is null)
      return Result<GetWalletBalanceResponse>.Failure(new DomainError("Wallet.NotFound", "Wallet not found."));

    var transactions = await _transactionRepository.GetByWalletIdAsync(query.WalletId);
    var incomingTransfers = await _transactionRepository.GetIncomingTransfersByWalletIdAsync(query.WalletId);

    var income = transactions.Where(t => t.Type == FinancialType.Income).Sum(t => t.Amount);
    var expense = transactions.Where(t => t.Type == FinancialType.Expense).Sum(t => t.Amount);
    var outgoingTransfers = transactions.Where(t => t.Type == FinancialType.Transfer).Sum(t => t.Amount);
    var incomingTransfersAmount = incomingTransfers.Sum(t => t.Amount);
    var balance = wallet.InitialBalance + income - expense - outgoingTransfers + incomingTransfersAmount;

    return Result<GetWalletBalanceResponse>.Success(new GetWalletBalanceResponse(query.WalletId, balance));
  }
}
