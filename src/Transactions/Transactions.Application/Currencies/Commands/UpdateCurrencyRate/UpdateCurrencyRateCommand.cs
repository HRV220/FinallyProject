using MediatR;
using Core.Domain.Common;
namespace Transactions.Application.Currencies.Commands.UpdateCurrencyRate;

public record UpdateCurrencyRateCommand(Guid Id, decimal Rate, decimal UnitRate) : IRequest<Result<bool>>;
