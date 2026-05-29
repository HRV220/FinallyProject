using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Transactions.Queries.GetTransactionById;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Result<GetTransactionByIdResponse>>
{
  private readonly ITransactionRepository _transactionRepository;

  public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository)
  {
    _transactionRepository = transactionRepository;
  }

  public async Task<Result<GetTransactionByIdResponse>> Handle(GetTransactionByIdQuery query, CancellationToken cancellationToken)
  {
    var transaction = await _transactionRepository.GetByIdAsync(query.Id);
    if (transaction is null)
      return Result<GetTransactionByIdResponse>.Failure(new DomainError("Transaction.NotFound", "Transaction not found."));

    return Result<GetTransactionByIdResponse>.Success(new GetTransactionByIdResponse(
      transaction.Id,
      transaction.WalletId,
      transaction.CategoryId,
      transaction.Type,
      transaction.Amount,
      transaction.Date,
      transaction.Description));
  }
}
