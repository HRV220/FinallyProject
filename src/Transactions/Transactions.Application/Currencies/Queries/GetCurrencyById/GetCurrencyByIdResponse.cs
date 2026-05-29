namespace Transactions.Application.Currencies.Queries.GetCurrencyById;

public record GetCurrencyByIdResponse(
  Guid Id,
  string Name,
  string Code,
  string NumericCode,
  int Nominal,
  decimal Rate,
  decimal UnitRate);
