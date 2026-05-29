using Transactions.Domain.Entities;
using Transactions.Domain.Enums;

namespace Transactions.Domain.Interfaces;

public interface ITransactionRepository
{
  Task CreateAsync(Transaction transaction);
  Task<Transaction?> GetByIdAsync(Guid id);
  Task<IEnumerable<Transaction>> GetByWalletIdAsync(Guid walletId);
  Task<IEnumerable<Transaction>> GetIncomingTransfersByWalletIdAsync(Guid walletId);
  Task UpdateAsync(Transaction transaction);
  Task DeleteAsync(Transaction transaction);
  Task<IEnumerable<CategoryTotal>> GetCategoryTotalsAsync(Guid profileId, FinancialType type, DateOnly? from, DateOnly? to);
  Task<IEnumerable<MonthlyTotal>> GetMonthlyTotalsAsync(Guid profileId, int year);
}

public class CategoryTotal
{
  public Guid? CategoryId { get; set; }
  public decimal Total { get; set; }
}

public class MonthlyTotal
{
  public int Month { get; set; }
  public decimal Income { get; set; }
  public decimal Expense { get; set; }
}
