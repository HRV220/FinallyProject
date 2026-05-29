using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Wallets.Commands.ChangeIcon;

public record ChangeIconCommand(Guid Id, string? NewIcon) : IRequest<Result<bool>>;
