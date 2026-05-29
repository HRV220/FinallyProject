using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Transactions.Queries.GetTransactionById;

public record GetTransactionByIdQuery(Guid Id) : IRequest<Result<GetTransactionByIdResponse>>;
