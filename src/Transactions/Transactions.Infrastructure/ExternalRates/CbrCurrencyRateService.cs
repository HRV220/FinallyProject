using System.Globalization;
using System.Xml.Linq;
using Transactions.Domain.Interfaces;

namespace Transactions.Infrastructure.ExternalRates;

public class CbrCurrencyRateService : ICbrCurrencyRateService
{
  private const string CbrUrl = "https://www.cbr.ru/scripts/XML_daily.asp";

  private readonly HttpClient _httpClient;

  public CbrCurrencyRateService(HttpClient httpClient)
  {
    _httpClient = httpClient;
  }

  public async Task<IEnumerable<CbrCurrencyRate>> GetRatesAsync(CancellationToken cancellationToken = default)
  {
    var xml = await _httpClient.GetStringAsync(CbrUrl, cancellationToken);
    var doc = XDocument.Parse(xml);

    return doc.Root!
      .Elements("Valute")
      .Select(v =>
      {
        var nominal = int.Parse(v.Element("Nominal")!.Value);
        var rate = decimal.Parse(v.Element("Value")!.Value, new CultureInfo("ru-RU"));
        var unitRate = decimal.Parse(v.Element("VunitRate")!.Value, new CultureInfo("ru-RU"));

        return new CbrCurrencyRate(
          Code: v.Element("CharCode")!.Value,
          NumericCode: v.Element("NumCode")!.Value,
          Name: v.Element("Name")!.Value,
          Nominal: nominal,
          Rate: rate,
          UnitRate: unitRate);
      })
      .ToList();
  }
}
