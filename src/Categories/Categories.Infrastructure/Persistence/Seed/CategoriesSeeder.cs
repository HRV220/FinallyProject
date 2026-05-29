using Categories.Domain.Entities;
using Categories.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Categories.Infrastructure.Persistence.Seed;

public static class CategoriesSeeder
{
  public static async Task SeedAsync(CategoriesDbContext context, CancellationToken ct = default)
  {
    if (await context.Categories.AnyAsync(ct))
      return;

    var income = new[]
    {
      Category.CreateSystem("Зарплата",         CategoryType.Income),
      Category.CreateSystem("Фриланс",          CategoryType.Income),
      Category.CreateSystem("Бонус",            CategoryType.Income),
      Category.CreateSystem("Дивиденды",        CategoryType.Income),
      Category.CreateSystem("Подарок",          CategoryType.Income),
      Category.CreateSystem("Возврат долга",    CategoryType.Income),
      Category.CreateSystem("Кэшбэк",           CategoryType.Income),
      Category.CreateSystem("Прочее",           CategoryType.Income),
    };

    var expense = new[]
    {
      Category.CreateSystem("Продукты питания", CategoryType.Expense),
      Category.CreateSystem("Рестораны и кафе", CategoryType.Expense),
      Category.CreateSystem("Транспорт",        CategoryType.Expense),
      Category.CreateSystem("Топливо",          CategoryType.Expense),
      Category.CreateSystem("Аренда жилья",     CategoryType.Expense),
      Category.CreateSystem("Коммунальные услуги", CategoryType.Expense),
      Category.CreateSystem("Связь и интернет", CategoryType.Expense),
      Category.CreateSystem("Одежда и обувь",   CategoryType.Expense),
      Category.CreateSystem("Здоровье",         CategoryType.Expense),
      Category.CreateSystem("Образование",      CategoryType.Expense),
      Category.CreateSystem("Развлечения",      CategoryType.Expense),
      Category.CreateSystem("Путешествия",      CategoryType.Expense),
      Category.CreateSystem("Подарки",          CategoryType.Expense),
      Category.CreateSystem("Прочее",           CategoryType.Expense),
    };

    foreach (var r in income.Concat(expense))
      if (r.IsSuccess)
        context.Categories.Add(r.Value!);

    await context.SaveChangesAsync(ct);
  }
}
