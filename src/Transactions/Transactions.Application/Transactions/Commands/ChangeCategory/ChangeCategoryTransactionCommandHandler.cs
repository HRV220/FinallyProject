using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Transactions.Commands.ChangeCategory;

public class ChangeCategoryTransactionCommandHandler : IRequestHandler<ChangeCategoryTransactionCommand, Result<bool>>
{
  private readonly ITransactionRepository _transactionRepository;

  public ChangeCategoryTransactionCommandHandler(ITransactionRepository transactionRepository)
  {
    _transactionRepository = transactionRepository;
  }

  public async Task<Result<bool>> Handle(ChangeCategoryTransactionCommand command, CancellationToken cancellationToken)
  {
    var transaction = await _transactionRepository.GetByIdAsync(command.Id);
    if (transaction is null)
      return Result<bool>.Failure(new DomainError("Transaction.NotFound", "Transaction not found."));

    transaction.ChangeCategory(command.NewCategoryId);
    await _transactionRepository.UpdateAsync(transaction);
    return Result<bool>.Success(true);
  }
}
