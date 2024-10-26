using System.Reflection;
using DotnetIntegrationTested.Common.Extensions.Http;
using DotnetIntegrationTested.Services.Http;
using DotnetIntegrationTested.Services.Http.Middleware;
using DotnetIntegrationTested.Services.SqlDb;

namespace DotnetIntegrationTested.AuthApi;

public sealed class Startup
{
  public void ConfigureServices(IServiceCollection services)
  {
    services.AddRouting();
    services.AddHttpContextAccessor();
    services.AddEndpoints(Assembly.GetExecutingAssembly());
    services.AddScoped<RequestPayloadValidatorService>();
    services.AddTransient<SqlConnectionFactory>();
  }

  public void Configure(IApplicationBuilder app)
  {
    app.UseExceptionMiddleware();
    app.UseRouting();
    app.UseEndpoints(cfg =>
    {
      cfg.MapGet("/", () => "Hi, this is the auth API.");
      cfg.MapEndpoints();
    });
  }
}
