using Microsoft.EntityFrameworkCore;
using Transactions.Domain.Entities;
using Transactions.Domain.Enums;
using Transactions.Domain.Interfaces;

namespace Transactions.Infrastructure.Persistence.Repositories;

public class TransactionRepository : ITransactionRepository
{
  private readonly TransactionsDbContext _context;

  public TransactionRepository(TransactionsDbContext context)
  {
    _context = context;
  }

  public async Task CreateAsync(Transaction transaction)
  {
    _context.Transactions.Add(transaction);
    await _context.SaveChangesAsync();
  }

  public Task<Transaction?> GetByIdAsync(Guid id) =>
    _context.Transactions.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);

  public async Task<IEnumerable<Transaction>> GetByWalletIdAsync(Guid walletId) =>
    await _context.Transactions.AsNoTracking()
      .Where(t => t.WalletId == walletId)
      .OrderByDescending(t => t.Date)
      .ToListAsync();

  public async Task<IEnumerable<Transaction>> GetIncomingTransfersByWalletIdAsync(Guid walletId) =>
    await _context.Transactions.AsNoTracking()
      .Where(t => t.ToWalletId == walletId)
      .ToListAsync();

  public async Task UpdateAsync(Transaction transaction)
  {
    await _context.Transactions
      .Where(t => t.Id == transaction.Id)
      .ExecuteUpdateAsync(s => s
        .SetProperty(t => t.Description, transaction.Description)
        .SetProperty(t => t.CategoryId, transaction.CategoryId)
        .SetProperty(t => t.UpdatedAt, DateTime.UtcNow));
  }

  public async Task DeleteAsync(Transaction transaction)
  {
    _context.Transactions.Remove(transaction);
    await _context.SaveChangesAsync();
  }

  // Analytics: группировка по CategoryId. Имя категории резолвится отдельным
  // запросом в Categories Service (на Этапе 4 — через HTTP-клиент в Application).
  public async Task<IEnumerable<CategoryTotal>> GetCategoryTotalsAsync(
    Guid profileId, FinancialType type, DateOnly? from, DateOnly? to)
  {
    var typeInt = (int)type;
    return await _context.Database.SqlQuery<CategoryTotal>($"""
        SELECT t."CategoryId" AS "CategoryId", SUM(t."Amount") AS "Total"
        FROM transactions."Transactions" t
        INNER JOIN transactions."Wallets" w ON t."WalletId" = w."Id"
        WHERE w."ProfileId" = {profileId}
          AND t."Type" = {typeInt}
          AND ({from}::date IS NULL OR t."Date" >= {from}::date)
          AND ({to}::date IS NULL OR t."Date" <= {to}::date)
        GROUP BY t."CategoryId"
        """)
        .ToListAsync();
  }

  public async Task<IEnumerable<MonthlyTotal>> GetMonthlyTotalsAsync(Guid profileId, int year)
  {
    var incomeInt = (int)FinancialType.Income;
    var expenseInt = (int)FinancialType.Expense;
    var transferInt = (int)FinancialType.Transfer;

    return await _context.Database.SqlQuery<MonthlyTotal>($"""
        SELECT
            EXTRACT(MONTH FROM t."Date")::int AS "Month",
            SUM(CASE WHEN t."Type" = {incomeInt}  THEN t."Amount" ELSE 0 END) AS "Income",
            SUM(CASE WHEN t."Type" = {expenseInt} THEN t."Amount" ELSE 0 END) AS "Expense"
        FROM transactions."Transactions" t
        INNER JOIN transactions."Wallets" w ON t."WalletId" = w."Id"
        WHERE w."ProfileId" = {profileId}
          AND EXTRACT(YEAR FROM t."Date") = {year}
          AND t."Type" <> {transferInt}
        GROUP BY EXTRACT(MONTH FROM t."Date")
        ORDER BY "Month"
        """)
        .ToListAsync();
  }
}
