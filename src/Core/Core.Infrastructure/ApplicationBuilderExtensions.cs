using Core.Infrastructure.Correlation;
using Core.Infrastructure.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Infrastructure;

public static class ApplicationBuilderExtensions
{

  public static IApplicationBuilder UsePlatformErrorHandling(this IApplicationBuilder app)
  {
    app.UseMiddleware<CorrelationIdMiddleware>();
    app.UseMiddleware<ProblemDetailsExceptionMiddleware>();
    return app;
  }


  public static IServiceCollection AddPlatformHttpEssentials(this IServiceCollection services)
  {
    services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    services.AddTransient<CorrelationIdHandler>();
    return services;
  }
}
