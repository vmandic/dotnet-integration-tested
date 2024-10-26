using System.Reflection;
using System.Text;
using DotnetIntegrationTested.Common.Extensions.Http;
using DotnetIntegrationTested.Common.Http;
using DotnetIntegrationTested.ExternalApis.Http.Wincher.V1;
using DotnetIntegrationTested.Services.Extensions.Mq;
using DotnetIntegrationTested.Services.Features.SeoChecker;
using DotnetIntegrationTested.Services.Http;
using DotnetIntegrationTested.Services.Http.Middleware;
using DotnetIntegrationTested.Services.Json;
using DotnetIntegrationTested.Services.MongoDb;
using DotnetIntegrationTested.Services.RedisDb;
using DotnetIntegrationTested.Services.SqlDb;
using Microsoft.IdentityModel.Tokens;

namespace DotnetIntegrationTested.HttpApi;

public class Startup
{
  private readonly IConfiguration _configuration;
  private AuthApiServerHandlerWrapper _backChannelHandlerWrapper = null!;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddRouting();
    services.AddHttpClientInterceptionForTestsOnly();
    services.AddWincherHttpClient();
    services.AddScoped<SeoCheckerService>();
    services.AddEndpoints(Assembly.GetExecutingAssembly());
    services.AddHttpContextAccessor();
    services.ConfigureJsonSerialization();
    services.AddScoped<RequestPayloadValidatorService>();
    services.AddTransient<SqlConnectionFactory>();
    services.AddSingleton<MongoDb>();
    services.AddScoped<RedisDb>();
    services.AddMassTransitWithRabbitMq();
    services.ConfigureHttpJsonOptions(options =>
    {
      options.SerializerOptions.PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance;
      options.SerializerOptions.DictionaryKeyPolicy = SnakeCaseNamingPolicy.Instance;
    });

    var jwtSecretKey =
      _configuration["Jwt:Secret"]
      ?? throw new InvalidOperationException("Required JWT secret key config value is missing");

    services.AddAuthorization();
    services
      .AddAuthentication("Bearer")
      .AddJwtBearer(
        "Bearer",
        options =>
        {
          // ref: https://stackoverflow.com/questions/64555221/xunit-integration-testing-with-identityserver-token-received-from-identityserve
          if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "IntegrationTests")
          {
            // NOTE: ensures that there is a back-channel communication with the IdP
            // provider in case the API services needs to handle token verification
            options.BackchannelHttpHandler = _backChannelHandlerWrapper.Handler;
          }

          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
          };
        }
      );
  }

  public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
  {
    if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "IntegrationTests")
    {
      _backChannelHandlerWrapper =
        app.ApplicationServices.GetRequiredService<AuthApiServerHandlerWrapper>();
    }

    app.UseExceptionMiddleware();
    app.UseAuthentication();
    app.UseRouting();
    app.UseAuthorization();
    app.UseEndpoints(cfg =>
    {
      cfg.MapGet("/", () => "Hi, this is a HTTP REST API and it is live and kicking.");
      cfg.MapEndpoints();
    });
  }
}
