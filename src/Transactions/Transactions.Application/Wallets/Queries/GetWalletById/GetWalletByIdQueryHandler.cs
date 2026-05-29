using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Wallets.Queries.GetWalletById;

public class GetWalletByIdQueryHandler : IRequestHandler<GetWalletByIdQuery, Result<GetWalletByIdResponse>>
{
  private readonly IWalletRepository _walletRepository;

  public GetWalletByIdQueryHandler(IWalletRepository walletRepository)
  {
    _walletRepository = walletRepository;
  }

  public async Task<Result<GetWalletByIdResponse>> Handle(GetWalletByIdQuery query, CancellationToken cancellationToken)
  {
    var wallet = await _walletRepository.GetWalletByIdAsync(query.Id);
    if (wallet is null)
      return Result<GetWalletByIdResponse>.Failure(new DomainError("Wallet.NotFound", "Wallet not found."));

    return Result<GetWalletByIdResponse>.Success(new GetWalletByIdResponse(
      wallet.Id,
      wallet.ProfileId,
      wallet.Name,
      wallet.Icon,
      wallet.SortOrder,
      wallet.CurrencyId,
      wallet.InitialBalance,
      wallet.Note,
      wallet.IsArchived));
  }
}
