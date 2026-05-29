namespace Transactions.Application.Currencies.Queries.GetAllCurrencies;

public record GetAllCurrenciesResponse(
  Guid Id,
  string Name,
  string Code,
  string NumericCode,
  int Nominal,
  decimal Rate,
  decimal UnitRate);
