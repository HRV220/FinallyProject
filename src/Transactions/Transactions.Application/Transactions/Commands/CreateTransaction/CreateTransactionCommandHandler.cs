using Core.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;
using Transactions.Application.Abstractions;
using Transactions.Domain.Entities;
using Transactions.Domain.Enums;
using Transactions.Domain.Interfaces;

namespace Transactions.Application.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Result<Guid>>
{
  private const decimal MaxAmount = 1_000_000_000_000m;

  private readonly ITransactionRepository _transactionRepository;
  private readonly IWalletRepository _walletRepository;
  private readonly IProfileChecker _profileChecker;
  private readonly ICategoryChecker _categoryChecker;
  private readonly ILogger<CreateTransactionCommandHandler> _logger;

  public CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IWalletRepository walletRepository,
    IProfileChecker profileChecker,
    ICategoryChecker categoryChecker,
    ILogger<CreateTransactionCommandHandler> logger)
  {
    _transactionRepository = transactionRepository;
    _walletRepository = walletRepository;
    _profileChecker = profileChecker;
    _categoryChecker = categoryChecker;
    _logger = logger;
  }

  public async Task<Result<Guid>> Handle(CreateTransactionCommand command, CancellationToken cancellationToken)
  {
    // Corner case: overflow guard
    if (command.Amount > MaxAmount)
      return Result<Guid>.Failure(new DomainError("Transaction.AmountTooLarge", $"Amount must be less than {MaxAmount}."));

    // 1. Source wallet exists
    var wallet = await _walletRepository.GetWalletByIdAsync(command.WalletId);
    if (wallet is null)
      return Result<Guid>.Failure(new DomainError("Transaction.WalletNotFound", "Wallet not found."));

    // 2. Source wallet is not archived (BR-W-3)
    if (wallet.IsArchived)
      return Result<Guid>.Failure(new DomainError("Wallet.Archived", "Cannot create transactions on archived wallet."));

    // 3. Profile exists and is active (HTTP to Users Service)
    try
    {
      if (!await _profileChecker.ExistsAsync(wallet.ProfileId, cancellationToken))
        return Result<Guid>.Failure(new DomainError("Profile.NotFoundOrInactive", "Profile not found or inactive."));
    }
    catch (UsersUnavailableException ex)
    {
      _logger.LogError(ex, "Users Service недоступен. Транзакция не будет создана.");
      return Result<Guid>.Failure(new DomainError("Dependency.UsersUnavailable", "Users Service is unavailable. Please try again later."));
    }

    // 4. Transfer-specific: ToWallet exists, not archived, same profile, same currency
    if (command.Type == FinancialType.Transfer)
    {
      if (!command.ToWalletId.HasValue)
        return Result<Guid>.Failure(new DomainError("Transaction.TransferRequiresDestination", "Transfer requires a destination wallet."));

      var toWallet = await _walletRepository.GetWalletByIdAsync(command.ToWalletId.Value);
      if (toWallet is null)
        return Result<Guid>.Failure(new DomainError("Transaction.DestinationWalletNotFound", "Destination wallet not found."));

      if (toWallet.IsArchived)
        return Result<Guid>.Failure(new DomainError("Wallet.Archived", "Cannot transfer to an archived wallet."));

      if (toWallet.ProfileId != wallet.ProfileId)
        return Result<Guid>.Failure(new DomainError("Transaction.CrossProfileTransfer", "Cannot transfer between wallets of different profiles."));

      if (toWallet.CurrencyId != wallet.CurrencyId)
        return Result<Guid>.Failure(new DomainError("Transaction.CurrencyMismatch", "Cannot transfer between wallets of different currencies."));
    }

    // 5. Category check (HTTP to Categories Service; graceful degradation)
    Guid? effectiveCategoryId = command.CategoryId;
    if (command.CategoryId.HasValue && command.CategoryId.Value != Guid.Empty)
    {
      var catCheck = await _categoryChecker.CheckAsync(command.CategoryId.Value, wallet.ProfileId, cancellationToken);
      switch (catCheck)
      {
        case CategoryCheckResult.NotFound:
          return Result<Guid>.Failure(new DomainError("Transaction.CategoryNotFound", "Category not found."));
        case CategoryCheckResult.Forbidden:
          return Result<Guid>.Failure(new DomainError("Category.NotOwned", "Category belongs to a different profile."));
        case CategoryCheckResult.Unknown:
          // graceful degradation: создаём без категории, чтобы не блокировать UX
          _logger.LogWarning("Categories Service недоступен — transaction создан с categoryId=null (profile={ProfileId})", wallet.ProfileId);
          effectiveCategoryId = null;
          break;
      }
    }

    // 6. Domain creation (доменные правила: amount > 0, тот же wallet и т.п.)
    var transaction = Transaction.Create(
      command.WalletId,
      command.Type,
      command.Amount,
      command.Date,
      effectiveCategoryId,
      command.Description,
      command.ToWalletId);

    if (transaction.IsFailure)
      return Result<Guid>.Failure(transaction.Error!);

    await _transactionRepository.CreateAsync(transaction.Value!);
    return Result<Guid>.Success(transaction.Value!.Id);
  }
}
