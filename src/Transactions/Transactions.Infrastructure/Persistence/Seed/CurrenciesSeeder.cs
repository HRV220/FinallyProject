using Microsoft.EntityFrameworkCore;
using Transactions.Domain.Entities;
using Transactions.Domain.Interfaces;

namespace Transactions.Infrastructure.Persistence.Seed;

public static class CurrenciesSeeder
{
  public static async Task SeedAsync(
    TransactionsDbContext context,
    ICbrCurrencyRateService cbrService,
    CancellationToken ct = default)
  {
    if (!await context.Currencies.AnyAsync(c => c.Code == "RUB", ct))
    {
      var rub = Currency.Create("Российский рубль", 1, 1.0000m, "643", "RUB", 1.0000m);
      if (rub.IsSuccess)
        context.Currencies.Add(rub.Value!);
      await context.SaveChangesAsync(ct);
    }

    IEnumerable<CbrCurrencyRate> rates;
    try
    {
      rates = await cbrService.GetRatesAsync(ct);
    }
    catch
    {
      // ЦБ недоступен — продолжаем работу с уже сохранёнными валютами
      return;
    }

    foreach (var rate in rates)
    {
      var existing = await context.Currencies.FirstOrDefaultAsync(c => c.Code == rate.Code, ct);
      if (existing is not null)
      {
        existing.UpdateRate(rate.Rate, rate.UnitRate);
      }
      else
      {
        var created = Currency.Create(rate.Name, rate.Nominal, rate.Rate, rate.NumericCode, rate.Code, rate.UnitRate);
        if (created.IsSuccess)
          context.Currencies.Add(created.Value!);
      }
    }

    await context.SaveChangesAsync(ct);
  }
}
