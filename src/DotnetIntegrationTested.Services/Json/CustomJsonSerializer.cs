using System.Text.Json;
using DotnetIntegrationTested.Common.Abstractions.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetIntegrationTested.Services.Json;

public static class CustomJsonSerializerExtensions
{
  public static IServiceCollection ConfigureJsonSerialization(this IServiceCollection services)
  {
    // ASP.NET JSON serializer
    services.Configure<JsonOptions>(opts =>
    {
      opts.SerializerOptions.PropertyNameCaseInsensitive = CustomJsonSerializer
        .InternalOptions
        .PropertyNameCaseInsensitive;

      opts.SerializerOptions.PropertyNamingPolicy = CustomJsonSerializer
        .InternalOptions
        .PropertyNamingPolicy;
    });

    // global serializer
    return services.AddSingleton<IJsonSerializer, CustomJsonSerializer>();
  }
}

public sealed class CustomJsonSerializer : IJsonSerializer
{
  internal static readonly JsonSerializerOptions InternalOptions =
    new()
    {
      PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
      PropertyNameCaseInsensitive = true,
    };

  public string Serialize(object obj) => JsonSerializer.Serialize(obj, InternalOptions);

  public T? Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json, InternalOptions);

  public JsonSerializerOptions Options => InternalOptions;
}
