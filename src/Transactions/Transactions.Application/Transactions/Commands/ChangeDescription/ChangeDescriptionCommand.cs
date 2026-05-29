using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Transactions.Commands.ChangeDescription;

public record ChangeDescriptionCommand(Guid Id, string? NewDescription) : IRequest<Result<bool>>;
