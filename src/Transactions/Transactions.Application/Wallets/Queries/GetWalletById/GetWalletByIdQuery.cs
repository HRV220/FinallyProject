using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Wallets.Queries.GetWalletById;

public record GetWalletByIdQuery(Guid Id) : IRequest<Result<GetWalletByIdResponse>>;
