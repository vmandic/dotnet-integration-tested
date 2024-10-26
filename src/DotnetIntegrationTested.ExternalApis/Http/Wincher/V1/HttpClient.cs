using System.Net;
using System.Net.Http.Json;
using DotnetIntegrationTested.Common.Abstractions.Json;
using DotnetIntegrationTested.Common.Http;
using DotnetIntegrationTested.ExternalApis.Http.Wincher.Auth.Endpoints.PostLogin;
using DotnetIntegrationTested.ExternalApis.Http.Wincher.V1.Endpoints.PostOnPageSeoChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nito.AsyncEx;

namespace DotnetIntegrationTested.ExternalApis.Http.Wincher.V1;

public static class ServiceCollectionExtensions
{
  internal const string AuthClientName = "wincher-auth";

  public static void AddWincherHttpClient(this IServiceCollection services)
  {
    // ref: https://www.wincher.com/docs/api
    services
      .AddHttpClient<WincherHttpClientV1>(
        (serviceProvider, client) =>
          client.BaseAddress = new Uri(
            serviceProvider.GetRequiredService<IConfiguration>()["WincherApi:Host"]!
          )
      )
      .AddHttpMessageHandler<TestInterceptionHandler>();

    services
      .AddHttpClient(
        AuthClientName,
        (serviceProvider, client) =>
        {
          client.BaseAddress = new Uri(
            serviceProvider.GetRequiredService<IConfiguration>()["WincherAuth:Host"]!
          );
        }
      )
      .AddHttpMessageHandler<TestInterceptionHandler>();
  }
}

public sealed class WincherHttpClientV1
{
  private readonly IJsonSerializer _jsonSerializer;
  private readonly IConfiguration _config;
  private readonly AsyncLock _lock = new();
  private string? _accessToken;

  internal HttpClient HttpClient { get; }

  internal HttpClient AuthHttpClient { get; }

  public WincherHttpClientV1(
    HttpClient httpHttpClient,
    IHttpClientFactory httpClientFactory,
    IJsonSerializer jsonSerializer,
    IConfiguration config
  )
  {
    HttpClient = httpHttpClient;
    AuthHttpClient = httpClientFactory.CreateClient(ServiceCollectionExtensions.AuthClientName);
    _jsonSerializer = jsonSerializer;
    _config = config;
  }

  public async Task<(
    PostOnPageSeoChecksRequestResponse? Result,
    HttpStatusCode HttpResonseCode
  )> PostOnPageSeoChecksAsync(PostOnPageSeoChecksRequest payload, CancellationToken ct)
  {
    var (accessToken, _) = await PostLoginAsync(ct);
    if (accessToken is null)
    {
      throw new UnauthorizedAccessException("API access unauthorized");
    }

    HttpClient.DefaultRequestHeaders.Remove("Authorization");
    HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

    var response = await HttpClient.PostAsJsonAsync(
      _config["WincherApi:Paths:PostOnPageSeoChecks"]!,
      payload,
      _jsonSerializer.Options,
      ct
    );
    HttpClient.DefaultRequestHeaders.Remove("Authorization");

    PostOnPageSeoChecksRequestResponse? result = null;
    if (response.IsSuccessStatusCode)
    {
      result = await response.Content.ReadFromJsonAsync<PostOnPageSeoChecksRequestResponse>(
        _jsonSerializer.Options,
        ct
      );
    }

    return (result, response.StatusCode);
  }

  private async Task<(string? AccessToken, HttpStatusCode? HttpResponseCode)> PostLoginAsync(
    CancellationToken ct = default
  )
  {
    using (await _lock.LockAsync(ct))
    {
      if (_accessToken is not null)
      {
        return (_accessToken, HttpStatusCode.OK);
      }

      using var request = new HttpRequestMessage(
        HttpMethod.Post,
        _config["WincherAuth:Paths:PostLogin"]
      );
      var collection = new List<KeyValuePair<string, string>>();
      collection.Add(new("grant_type", _config["WincherAuth:GrantType"]!));
      collection.Add(new("client_id", _config["WincherAuth:ClientId"]!));
      collection.Add(new("scope", _config["WincherAuth:Scope"]!));
      collection.Add(new("username", _config["WincherAuth:Username"]!));
      collection.Add(new("password", _config["WincherAuth:Password"]!));
      var content = new FormUrlEncodedContent(collection);
      request.Content = content;

      var response = await AuthHttpClient.SendAsync(request, ct);
      if (!response.IsSuccessStatusCode)
      {
        return (null, response.StatusCode);
      }

      var tokenModel = await response.Content.ReadFromJsonAsync<PostLoginResponse>(
        cancellationToken: ct
      );

      if (tokenModel is null)
      {
        return (null, null);
      }

      _accessToken = tokenModel.AccessToken;
      return (tokenModel.AccessToken, response.StatusCode);
    }
  }
}
