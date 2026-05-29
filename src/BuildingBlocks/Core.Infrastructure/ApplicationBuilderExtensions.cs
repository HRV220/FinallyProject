using Core.Infrastructure.Correlation;
using Core.Infrastructure.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Infrastructure;

public static class ApplicationBuilderExtensions
{
  /// <summary>
  /// Подключает CorrelationId + ProblemDetails. Регистрируется в самом начале pipeline.
  /// </summary>
  public static IApplicationBuilder UsePlatformErrorHandling(this IApplicationBuilder app)
  {
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ProblemDetailsExceptionMiddleware>();
    return app;
  }

  /// <summary>
  /// Регистрирует IHttpContextAccessor + DelegatingHandler для проброса CorrelationId
  /// исходящим вызовам через HttpClient/Refit. Подключается перед AddRefitClient.
  /// </summary>
  public static IServiceCollection AddPlatformHttpEssentials(this IServiceCollection services)
  {
    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddTransient<CorrelationIdHandler>();
    return services;
  }
}
