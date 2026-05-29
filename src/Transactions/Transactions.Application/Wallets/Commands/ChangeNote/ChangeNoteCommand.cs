using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Wallets.Commands.ChangeNote;

public record ChangeNoteCommand(Guid Id, string? NewNote) : IRequest<Result<bool>>;
