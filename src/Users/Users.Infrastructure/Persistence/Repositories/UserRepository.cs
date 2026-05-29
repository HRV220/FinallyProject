using Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Interfaces;
using Users.Domain.ValueObject;

namespace Users.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
  private readonly UserDbContext _context;
  public UserRepository(UserDbContext context)
  {
    _context = context;
  }

  public async Task AddUserAsync(User user)
  {
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
  }

  public async Task DeleteUserAsync(User user)
  {
    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
  }

  public async Task<User?> GetByEmailAsync(Email email)
  {
    return await _context.Users.FirstOrDefaultAsync(u => u.Email.Value == email.Value);
  }

  public async Task<User?> GetByIdAsync(Guid id)
  {
    return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
  }

  public async Task UpdateUserAsync(User user)
  {
    await _context.Users
      .Where(u => u.Id == user.Id)
      .ExecuteUpdateAsync(s => s
        .SetProperty(u => u.Email.Value, user.Email.Value)
        .SetProperty(u => u.PasswordHash.Value, user.PasswordHash.Value)
        .SetProperty(u => u.FirstName, user.FirstName)
        .SetProperty(u => u.LastName, user.LastName)
        .SetProperty(u => u.PhoneNumber, user.PhoneNumber)
        .SetProperty(u => u.UpdatedAt, DateTime.UtcNow)
        );
  }
}