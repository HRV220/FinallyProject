using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Transactions.Queries.GetTransactionsByWalletId;

public class GetTransactionsByWalletIdQueryHandler : IRequestHandler<GetTransactionsByWalletIdQuery, Result<IEnumerable<GetTransactionsByWalletIdResponse>>>
{
  private readonly ITransactionRepository _transactionRepository;

  public GetTransactionsByWalletIdQueryHandler(ITransactionRepository transactionRepository)
  {
    _transactionRepository = transactionRepository;
  }

  public async Task<Result<IEnumerable<GetTransactionsByWalletIdResponse>>> Handle(GetTransactionsByWalletIdQuery query, CancellationToken cancellationToken)
  {
    var transactions = await _transactionRepository.GetByWalletIdAsync(query.WalletId);

    var response = transactions.Select(t => new GetTransactionsByWalletIdResponse(
      t.Id,
      t.WalletId,
      t.CategoryId,
      t.Type,
      t.Amount,
      t.Date,
      t.Description));

    return Result<IEnumerable<GetTransactionsByWalletIdResponse>>.Success(response);
  }
}
