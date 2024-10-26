using System.Text.Json.Serialization;

namespace DotnetIntegrationTested.ExternalApis.Http.Wincher.Auth.Endpoints.PostLogin;

public sealed record PostLoginResponse(
  [property: JsonPropertyName("access_token")] string AccessToken
);
