using Categories.Application.Abstractions;
using Categories.Domain.Interfaces;
using Categories.Infrastructure.Clients;
using Categories.Infrastructure.Persistence;
using Categories.Infrastructure.Persistence.Repositories;
using Categories.Infrastructure.Services;
using Core.Infrastructure;
using Core.Infrastructure.Correlation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Refit;

namespace Categories.Infrastructure;

public static class DependencyInjection
{
  public static IServiceCollection AddCategoriesModule(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<CategoriesDbContext>(options =>
      options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection"),
        npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", CategoriesDbContext.Schema)));

    services.AddScoped<ICategoryRepository, CategoryRepository>();
    services.AddScoped<IProfileChecker, HttpProfileChecker>();

    services.AddPlatformHttpEssentials();

    var usersBaseUrl = configuration["UsersService:BaseUrl"]
      ?? throw new InvalidOperationException("UsersService:BaseUrl is required.");

    services
      .AddRefitClient<IUsersServiceClient>()
      .ConfigureHttpClient(c =>
      {
        c.BaseAddress = new Uri(usersBaseUrl);
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

    return services;
  }
}
