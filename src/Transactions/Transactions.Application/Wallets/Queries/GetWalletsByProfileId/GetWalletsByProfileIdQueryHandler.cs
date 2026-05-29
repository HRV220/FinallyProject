using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Wallets.Queries.GetWalletsByProfileId;

public class GetWalletsByProfileIdQueryHandler : IRequestHandler<GetWalletsByProfileIdQuery, Result<IEnumerable<GetWalletsByProfileIdResponse>>>
{
  private readonly IWalletRepository _walletRepository;

  public GetWalletsByProfileIdQueryHandler(IWalletRepository walletRepository)
  {
    _walletRepository = walletRepository;
  }

  public async Task<Result<IEnumerable<GetWalletsByProfileIdResponse>>> Handle(GetWalletsByProfileIdQuery query, CancellationToken cancellationToken)
  {
    var wallets = await _walletRepository.GetWalletsByProfileIdAsync(query.ProfileId);
    var response = wallets.Select(w => new GetWalletsByProfileIdResponse(
      w.Id,
      w.ProfileId,
      w.Name,
      w.Icon,
      w.SortOrder,
      w.CurrencyId,
      w.InitialBalance,
      w.Note,
      w.IsArchived));

    return Result<IEnumerable<GetWalletsByProfileIdResponse>>.Success(response);
  }
}
