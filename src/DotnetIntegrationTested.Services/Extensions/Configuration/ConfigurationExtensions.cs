using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DotnetIntegrationTested.Services.Extensions.Configuration;

public static class ConfigurationExtensions
{
  public static HostApplicationBuilder AddConfiguration(
    this HostApplicationBuilder builder,
    Dictionary<string, string>? overrides = null
  )
  {
    ConfigureConfigurationBuilder(
      builder.Environment.EnvironmentName,
      builder.Configuration,
      overrides
    );
    return builder;
  }

  public static IWebHostBuilder AddConfiguration(
    this IWebHostBuilder builder,
    Dictionary<string, string>? overrides = null
  ) =>
    builder.ConfigureAppConfiguration(
      (hostBuilderContext, config) =>
      {
        var env = hostBuilderContext.HostingEnvironment.EnvironmentName;
        ConfigureConfigurationBuilder(env, config, overrides);
      }
    );

  public static void ConfigureConfigurationBuilder(
    string dotnetApplicationEnvironment,
    IConfigurationBuilder configurationBuilder,
    Dictionary<string, string>? overrides = null
  )
  {
    configurationBuilder.Sources.Clear();
    configurationBuilder.Properties.Clear();
    configurationBuilder.AddJsonFile("appsettings.json", false);

    if (!string.IsNullOrWhiteSpace(dotnetApplicationEnvironment))
    {
      configurationBuilder.AddJsonFile($"appsettings.{dotnetApplicationEnvironment}.json", true);
    }

    configurationBuilder.AddJsonFile("appsettings.localhost.json", true);
    configurationBuilder.AddEnvironmentVariables();

    if (overrides is not null)
    {
      configurationBuilder.AddInMemoryCollection(overrides!);
    }
  }
}
