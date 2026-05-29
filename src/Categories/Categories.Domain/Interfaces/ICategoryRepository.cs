using Categories.Domain.Entities;

namespace Categories.Domain.Interfaces;

public interface ICategoryRepository
{
  Task CreateAsync(Category category);
  Task<Category?> GetByIdAsync(Guid id);
  Task<IEnumerable<Category>> GetByProfileIdAsync(Guid profileId);
  Task<IEnumerable<Category>> GetSystemCategoriesAsync();
  Task UpdateAsync(Category category);
  Task DeleteAsync(Category category);
}
