using Users.Domain.Entities;

namespace Users.Domain.Interfaces;
public interface IProfileRepository
{
  Task<Profile?> GetByIdProfileAsync(Guid id);
  Task<IEnumerable<Profile>> GetByUserIdProfilesAsync(Guid userId);
  Task DeleteAsync(Profile profile);
  Task UpdateProfileAsync(Profile profile);
  Task AddProfileAsync(Profile profile);
}