using Transactions.Domain.Entities;
using Transactions.Domain.Interfaces;
using Transactions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Transactions.Infrastructure.Persistence.Repositories;

public class CurrencyRepository : ICurrencyRepository
{
  private readonly TransactionsDbContext _context;

  public CurrencyRepository(TransactionsDbContext context)
  {
    _context = context;
  }

  public async Task<Currency?> GetByIdAsync(Guid id)
  {
    return await _context.Currencies.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
  }

  public async Task<Currency?> GetByCodeAsync(string code)
  {
    return await _context.Currencies.AsNoTracking().FirstOrDefaultAsync(c => c.Code == code.ToUpper());
  }

  public async Task<IEnumerable<Currency>> GetAllAsync()
  {
    return await _context.Currencies.AsNoTracking().ToListAsync();
  }

  public async Task AddAsync(Currency currency)
  {
    _context.Currencies.Add(currency);
    await _context.SaveChangesAsync();
  }

  public async Task UpdateAsync(Currency currency)
  {
    await _context.Currencies
      .Where(c => c.Id == currency.Id)
      .ExecuteUpdateAsync(s => s
        .SetProperty(c => c.Rate, currency.Rate)
        .SetProperty(c => c.UnitRate, currency.UnitRate)
        .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));
  }
}
