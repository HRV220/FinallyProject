using Users.Domain.ValueObject;
using Users.Domain.Entities;


namespace Users.Domain.Interfaces;
public interface IUserRepository
{
  Task<User?> GetByIdAsync(Guid id);
  Task<User?> GetByEmailAsync(Email email);
  Task AddUserAsync(User user);
  Task UpdateUserAsync(User user);
  Task DeleteUserAsync(User user);
}