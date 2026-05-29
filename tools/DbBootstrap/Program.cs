using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Users.Infrastructure.Persistence;
using Categories.Infrastructure.Persistence;
using Categories.Infrastructure.Persistence.Seed;
using Transactions.Infrastructure.Persistence;
using Transactions.Infrastructure.Persistence.Seed;
using Transactions.Domain.Interfaces;
using Transactions.Infrastructure.ExternalRates;
using Reports.Infrastructure.Persistence;

string host = Env("PGHOST", "localhost");
string port = Env("PGPORT", "5432");
string user = Env("PGUSER", "postgres");
string pass = Env("PGPASSWORD", "postgres");
string targetDb = Env("PGDATABASE", "finance_tracker");

string adminCs = $"Host={host};Port={port};Username={user};Password={pass};Database=postgres";

Console.WriteLine($"[bootstrap] Target database: {targetDb}");
Console.WriteLine($"[bootstrap] Host: {host}:{port}, User: {user}");

await CreateDatabaseIfNotExistsAsync(adminCs, targetDb);

await CreateSchemasAsync(host, port, user, pass, targetDb, new[] { "users", "categories", "transactions", "reports" });

var services = new ServiceCollection();

services.AddDbContext<UserDbContext>(options =>
    options.UseNpgsql($"Host={host};Port={port};Database={targetDb};Username={user};Password={pass};Search Path=users"));

services.AddDbContext<CategoriesDbContext>(options =>
    options.UseNpgsql($"Host={host};Port={port};Database={targetDb};Username={user};Password={pass};Search Path=categories"));

services.AddDbContext<TransactionsDbContext>(options =>
    options.UseNpgsql($"Host={host};Port={port};Database={targetDb};Username={user};Password={pass};Search Path=transactions"));

services.AddDbContext<ReportsDbContext>(options =>
    options.UseNpgsql($"Host={host};Port={port};Database={targetDb};Username={user};Password={pass};Search Path=reports"));

services.AddHttpClient<ICbrCurrencyRateService, CbrCurrencyRateService>();

var serviceProvider = services.BuildServiceProvider();

using (var scope = serviceProvider.CreateScope())
{
  Console.WriteLine("[bootstrap] Applying migrations for UserDbContext...");
  var userDb = scope.ServiceProvider.GetRequiredService<UserDbContext>();
  await userDb.Database.MigrateAsync();
  Console.WriteLine("[bootstrap]   ok — users schema updated.");

  Console.WriteLine("[bootstrap] Applying migrations for CategoriesDbContext...");
  var catDb = scope.ServiceProvider.GetRequiredService<CategoriesDbContext>();
  await catDb.Database.MigrateAsync();
  Console.WriteLine("[bootstrap] Seeding default categories...");
  await CategoriesSeeder.SeedAsync(catDb);
  Console.WriteLine("[bootstrap]   ok — categories schema updated.");

  Console.WriteLine("[bootstrap] Applying migrations for TransactionsDbContext...");
  var txDb = scope.ServiceProvider.GetRequiredService<TransactionsDbContext>();
  await txDb.Database.MigrateAsync();
  Console.WriteLine("[bootstrap] Seeding currency rates from CBR...");
  var cbrService = scope.ServiceProvider.GetRequiredService<ICbrCurrencyRateService>();
  await CurrenciesSeeder.SeedAsync(txDb, cbrService);
  Console.WriteLine("[bootstrap]   ok — transactions schema updated.");

  Console.WriteLine("[bootstrap] Applying migrations for ReportsDbContext...");
  var reportDb = scope.ServiceProvider.GetRequiredService<ReportsDbContext>();
  await reportDb.Database.MigrateAsync();
  Console.WriteLine("[bootstrap]   ok — reports schema updated.");
}

Console.WriteLine("[bootstrap] Database bootstrapping successfully completed.");
return 0;

static string Env(string key, string fallback) => Environment.GetEnvironmentVariable(key) is { Length: > 0 } v ? v : fallback;

static async Task CreateDatabaseIfNotExistsAsync(string adminCs, string dbName)
{
  await using var conn = new NpgsqlConnection(adminCs);
  await conn.OpenAsync();

  await using (var check = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = @name", conn))
  {
    check.Parameters.AddWithValue("name", dbName);
    var exists = await check.ExecuteScalarAsync();
    if (exists != null)
    {
      Console.WriteLine($"[bootstrap] Database \"{dbName}\" already exists.");
      return;
    }
  }

  await using var create = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\" ENCODING 'UTF8'", conn);
  await create.ExecuteNonQueryAsync();
  Console.WriteLine($"[bootstrap] Database \"{dbName}\" successfully created.");
}

static async Task CreateSchemasAsync(string host, string port, string user, string pass, string dbName, string[] schemas)
{
  var cs = $"Host={host};Port={port};Username={user};Password={pass};Database={dbName}";
  await using var conn = new NpgsqlConnection(cs);
  await conn.OpenAsync();
  foreach (var schema in schemas)
  {
    await using var cmd = new NpgsqlCommand($"CREATE SCHEMA IF NOT EXISTS \"{schema}\"", conn);
    await cmd.ExecuteNonQueryAsync();
    Console.WriteLine($"[bootstrap] Schema \"{schema}\" ensured.");
  }
}