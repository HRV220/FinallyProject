using MediatR;
using Core.Domain.Common;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Transactions.Commands.ChangeDescription;

public class ChangeDescriptionCommandHandler : IRequestHandler<ChangeDescriptionCommand, Result<bool>>
{
  private readonly ITransactionRepository _transactionRepository;

  public ChangeDescriptionCommandHandler(ITransactionRepository transactionRepository)
  {
    _transactionRepository = transactionRepository;
  }

  public async Task<Result<bool>> Handle(ChangeDescriptionCommand command, CancellationToken cancellationToken)
  {
    var transaction = await _transactionRepository.GetByIdAsync(command.Id);
    if (transaction is null)
      return Result<bool>.Failure(new DomainError("Transaction.NotFound", "Transaction not found."));

    var result = transaction.ChangeDescription(command.NewDescription);
    if (result.IsFailure)
      return result;

    await _transactionRepository.UpdateAsync(transaction);
    return Result<bool>.Success(true);
  }
}
