using Categories.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Reports.Infrastructure.Persistence;
using Transactions.Infrastructure.Persistence;
using Users.Infrastructure.Persistence;

string host = Env("PGHOST", "localhost");
string port = Env("PGPORT", "5432");
string user = Env("PGUSER", "postgres");
string pass = Env("PGPASSWORD", "postgres");
string targetDb = Env("PGDATABASE", "FinanceTrackerNew");

string adminCs = $"Host={host};Port={port};Username={user};Password={pass};Database=postgres";

Console.WriteLine($"[bootstrap] target database: {targetDb}");
Console.WriteLine($"[bootstrap] host: {host}:{port}, user: {user}");

await CreateDatabaseIfNotExistsAsync(adminCs, targetDb);
await CreateSchemasAsync(host, port, user, pass, targetDb, new[] { "users", "categories", "transactions", "reports" });

await MigrateAsync<UserDbContext>($"Host={host};Port={port};Database={targetDb};Username={user};Password={pass};Search Path=users", "users");
await MigrateAsync<CategoriesDbContext>($"Host={host};Port={port};Database={targetDb};Username={user};Password={pass};Search Path=categories", "categories");
await MigrateAsync<TransactionsDbContext>($"Host={host};Port={port};Database={targetDb};Username={user};Password={pass};Search Path=transactions", "transactions");
await MigrateAsync<ReportsDbContext>($"Host={host};Port={port};Database={targetDb};Username={user};Password={pass};Search Path=reports", "reports");

Console.WriteLine("[bootstrap] done");
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
      Console.WriteLine($"[bootstrap] database \"{dbName}\" already exists — skipping CREATE");
      return;
    }
  }

  await using var create = new NpgsqlCommand($"CREATE DATABASE \"{dbName}\" ENCODING 'UTF8'", conn);
  await create.ExecuteNonQueryAsync();
  Console.WriteLine($"[bootstrap] database \"{dbName}\" created");
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
    Console.WriteLine($"[bootstrap] schema \"{schema}\" ensured");
  }
}

static async Task MigrateAsync<TContext>(string connectionString, string schema) where TContext : DbContext
{
  var options = new DbContextOptionsBuilder<TContext>()
    .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema))
    .Options;
  await using var ctx = (TContext)Activator.CreateInstance(typeof(TContext), options)!;
  Console.WriteLine($"[bootstrap] applying migrations: {typeof(TContext).Name} (schema={schema})");
  await ctx.Database.MigrateAsync();
  Console.WriteLine($"[bootstrap]   ok — {typeof(TContext).Name}");
}
