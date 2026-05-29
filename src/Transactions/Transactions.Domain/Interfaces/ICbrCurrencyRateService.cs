namespace Transactions.Domain.Interfaces;

public interface ICbrCurrencyRateService
{
  Task<IEnumerable<CbrCurrencyRate>> GetRatesAsync(CancellationToken cancellationToken = default);
}

public record CbrCurrencyRate(string Code, string NumericCode, string Name, int Nominal, decimal Rate, decimal UnitRate);
