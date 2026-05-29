using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Transactions.Queries.GetWalletBalance;

public record GetWalletBalanceQuery(Guid WalletId) : IRequest<Result<GetWalletBalanceResponse>>;
