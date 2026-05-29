using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Transactions.Commands.ChangeCategory;

public record ChangeCategoryTransactionCommand(Guid Id, Guid? NewCategoryId) : IRequest<Result<bool>>;
