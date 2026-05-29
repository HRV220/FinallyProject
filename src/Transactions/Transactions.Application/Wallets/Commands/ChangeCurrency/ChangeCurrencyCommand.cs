using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Wallets.Commands.ChangeCurrency;

public record ChangeCurrencyCommand(Guid Id, Guid NewCurrencyId) : IRequest<Result<bool>>;
