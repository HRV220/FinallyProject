using Core.Infrastructure;
using Core.Infrastructure.Correlation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;
using Transactions.Application.Abstractions;
using Transactions.Domain.Interfaces;
using Transactions.Infrastructure.Clients;
using Transactions.Infrastructure.ExternalRates;
using Transactions.Infrastructure.Persistence;
using Transactions.Infrastructure.Persistence.Repositories;
using Transactions.Infrastructure.Services;

namespace Transactions.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddTransactionsModule(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<TransactionsDbContext>(options =>
      options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", TransactionsDbContext.Schema)));

    services.AddScoped<IWalletRepository, WalletRepository>();
    services.AddScoped<ICurrencyRepository, CurrencyRepository>();
    services.AddScoped<ITransactionRepository, TransactionRepository>();
    services.AddScoped<IProfileChecker, HttpProfileChecker>();
    services.AddScoped<ICategoryChecker, HttpCategoryChecker>();

    services.AddHttpClient<ICbrCurrencyRateService, CbrCurrencyRateService>();

    services.AddPlatformHttpEssentials();

    AddResilientRefitClient<IUsersServiceClient>(services,
      configuration["UsersService:BaseUrl"]
        ?? throw new InvalidOperationException("UsersService:BaseUrl is required."));

    AddResilientRefitClient<ICategoriesServiceClient>(services,
      configuration["CategoriesService:BaseUrl"]
        ?? throw new InvalidOperationException("CategoriesService:BaseUrl is required."));

    return services;
  }

  private static void AddResilientRefitClient<TClient>(IServiceCollection services, string baseUrl) where TClient : class
  {
    services
      .AddRefitClient<TClient>()
      .ConfigureHttpClient(c =>
      {
        c.BaseAddress = new Uri(baseUrl);
        c.Timeout = TimeSpan.FromSeconds(10);
      })
      .AddHttpMessageHandler<CorrelationIdHandler>()
      .AddStandardResilienceHandler(options =>
      {
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(2);
        options.Retry.MaxRetryAttempts = 2;
        options.Retry.Delay = TimeSpan.FromMilliseconds(250);
        options.Retry.BackoffType = DelayBackoffType.Exponential;
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
      });
  }
}
