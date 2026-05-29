using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Transactions.Queries.GetTransactionsByWalletId;

public record GetTransactionsByWalletIdQuery(Guid WalletId) : IRequest<Result<IEnumerable<GetTransactionsByWalletIdResponse>>>;
