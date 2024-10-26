using DotnetIntegrationTested.Services.Extensions.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DotnetIntegrationTested.Services.Http;

public static class WebHostTools
{
  public static IWebHostBuilder CreateDefaultBuilder<TStartup>(
    string[] args,
    Dictionary<string, string>? configOverrides = null
  )
    where TStartup : class =>
    WebHost
      .CreateDefaultBuilder(args)
      .AddConfiguration(configOverrides)
      .UseDefaultServiceProvider(
        (_, opts) =>
        {
          opts.ValidateScopes = true;
          opts.ValidateOnBuild = true;
        }
      )
      .UseStartup<TStartup>();
}
