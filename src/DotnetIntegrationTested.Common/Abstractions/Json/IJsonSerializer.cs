using System.Text.Json;

namespace DotnetIntegrationTested.Common.Abstractions.Json;

public interface IJsonSerializer
{
  string Serialize(object obj);

  T? Deserialize<T>(string json);

  JsonSerializerOptions Options { get; }
}
