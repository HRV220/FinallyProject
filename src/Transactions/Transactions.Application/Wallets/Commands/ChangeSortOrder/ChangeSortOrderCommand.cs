using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Wallets.Commands.ChangeSortOrder;

public record ChangeSortOrderCommand(Guid Id, int NewSortOrder) : IRequest<Result<bool>>;
