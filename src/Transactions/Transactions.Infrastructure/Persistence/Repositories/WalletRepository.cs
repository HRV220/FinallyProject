using Transactions.Domain.Entities;
using Transactions.Domain.Interfaces;
using Transactions.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Transactions.Infrastructure.Persistence.Repositories;

public class WalletRepository : IWalletRepository
{
  private readonly TransactionsDbContext _context;

  public WalletRepository(TransactionsDbContext context)
  {
    _context = context;
  }

  public async Task CreateAsync(Wallet wallet)
  {
    _context.Wallets.Add(wallet);
    await _context.SaveChangesAsync();
  }

  public async Task<Wallet?> GetWalletByIdAsync(Guid id)
  {
    return await _context.Wallets.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
  }

  public async Task<IEnumerable<Wallet>> GetWalletsByProfileIdAsync(Guid profileId)
  {
    return await _context.Wallets.AsNoTracking().Where(w => w.ProfileId == profileId).ToListAsync();
  }

  public async Task UpdateAsync(Wallet wallet)
  {
    await _context.Wallets
      .Where(w => w.Id == wallet.Id)
      .ExecuteUpdateAsync(s => s
        .SetProperty(w => w.Name, wallet.Name)
        .SetProperty(w => w.Icon, wallet.Icon)
        .SetProperty(w => w.SortOrder, wallet.SortOrder)
        .SetProperty(w => w.CurrencyId, wallet.CurrencyId)
        .SetProperty(w => w.Note, wallet.Note)
        .SetProperty(w => w.IsArchived, wallet.IsArchived)
        .SetProperty(w => w.UpdatedAt, DateTime.UtcNow));
  }

  public async Task DeleteAsync(Guid id)
  {
    var wallet = await _context.Wallets.FindAsync(id);
    if (wallet is null) return;
    _context.Wallets.Remove(wallet);
    await _context.SaveChangesAsync();
  }
}
