using Transactions.Domain.Enums;
using Core.Domain.Entities;
using Core.Domain.Common;

namespace Transactions.Domain.Entities;

public class Transaction : BaseEntity
{
  public Guid WalletId { get; private set; }
  public Wallet Wallet { get; private set; } = null!;
  public Guid? ToWalletId { get; private set; }
  public Wallet? ToWallet { get; private set; }
  public Guid? CategoryId { get; private set; }
  public FinancialType Type { get; private set; }
  public decimal Amount { get; private set; }
  public DateOnly Date { get; private set; }
  public string? Description { get; private set; }

  private Transaction() : base() { }

  private Transaction(Guid id, Guid walletId, Guid? toWalletId, Guid? categoryId, FinancialType type, decimal amount, DateOnly date, string? description) : base(id)
  {
    WalletId = walletId;
    ToWalletId = toWalletId;
    CategoryId = categoryId;
    Type = type;
    Amount = amount;
    Date = date;
    Description = description;
  }

  public static Result<Transaction> Create(Guid walletId, FinancialType type, decimal amount, DateOnly date, Guid? categoryId = null, string? description = null, Guid? toWalletId = null)
  {
    if (walletId == Guid.Empty)
      return Result<Transaction>.Failure(new DomainError("Transaction.InvalidWalletId", "WalletId is required."));

    if (amount <= 0)
      return Result<Transaction>.Failure(new DomainError("Transaction.InvalidAmount", "Amount must be greater than 0."));

    if (type == FinancialType.Transfer)
    {
      if (toWalletId is null || toWalletId == Guid.Empty)
        return Result<Transaction>.Failure(new DomainError("Transaction.TransferRequiresDestination", "Transfer requires a destination wallet."));

      if (toWalletId == walletId)
        return Result<Transaction>.Failure(new DomainError("Transaction.SameWalletTransfer", "Cannot transfer to the same wallet."));
    }

    if (description?.Length > 500)
      return Result<Transaction>.Failure(new DomainError("Transaction.InvalidDescription", "Description must be 500 characters or less."));

    return Result<Transaction>.Success(new Transaction(Guid.NewGuid(), walletId, toWalletId, categoryId, type, amount, date, description));
  }

  public Result<bool> ChangeDescription(string? newDescription)
  {
    if (newDescription?.Length > 500)
      return Result<bool>.Failure(new DomainError("Transaction.InvalidDescription", "Description must be 500 characters or less."));

    Description = newDescription;
    SetUpdated();
    return Result<bool>.Success(true);
  }

  public void ChangeCategory(Guid? newCategoryId)
  {
    CategoryId = newCategoryId;
    SetUpdated();
  }
}
