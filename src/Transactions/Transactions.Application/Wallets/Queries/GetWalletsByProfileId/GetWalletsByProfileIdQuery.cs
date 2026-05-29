using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Wallets.Queries.GetWalletsByProfileId;

public record GetWalletsByProfileIdQuery(Guid ProfileId) : IRequest<Result<IEnumerable<GetWalletsByProfileIdResponse>>>;
