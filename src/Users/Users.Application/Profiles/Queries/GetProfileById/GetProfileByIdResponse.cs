namespace Users.Application.Profiles.Queries.GetProfileById;

public record GetProfileByIdResponse(Guid Id, Guid UserId, string Name, bool IsActive);