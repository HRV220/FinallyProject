using Transactions.Domain.Entities;

namespace Transactions.Domain.Interfaces;

public interface ICurrencyRepository
{
  Task<Currency?> GetByIdAsync(Guid id);
  Task<Currency?> GetByCodeAsync(string code);
  Task<IEnumerable<Currency>> GetAllAsync();
  Task AddAsync(Currency currency);
  Task UpdateAsync(Currency currency);
}
