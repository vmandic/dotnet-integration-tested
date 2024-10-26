using System.Text;
using System.Text.Json;
using DotnetIntegrationTested.Services.Json;

namespace DotnetIntegrationTested.IntegrationTests.Extensions;

public static class ObjectExtensions
{
  public static StringContent AsJsonHttpContent(
    this object value,
    JsonSerializerOptions? jsonSerializerOptions = null
  )
  {
    jsonSerializerOptions ??= CustomJsonSerializer.InternalOptions;
    var json = JsonSerializer.Serialize(value, jsonSerializerOptions);
    return new StringContent(json, Encoding.UTF8, "application/json");
  }
}
