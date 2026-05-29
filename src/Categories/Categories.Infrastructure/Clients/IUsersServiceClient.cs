using Refit;

namespace Categories.Infrastructure.Clients;

public interface IUsersServiceClient
{
  [Get("/api/profiles/{id}")]
  Task<ProfileDto> GetProfileAsync(Guid id, CancellationToken ct = default);
}

public record ProfileDto(Guid Id, Guid UserId, string Name, bool IsActive);
