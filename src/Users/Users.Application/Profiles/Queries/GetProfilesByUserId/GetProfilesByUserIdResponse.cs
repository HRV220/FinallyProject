namespace Users.Application.Profiles.Queries.GetProfilesByUserId;

public record GetProfilesByUserIdResponse(Guid Id, string Name, bool IsActive);
