namespace Users.Application.Users.Queries.GetUserById;

public record GetUserByIdResponse(Guid Id, string Email, string? FirstName, string? LastName, string? PhoneNumber);
