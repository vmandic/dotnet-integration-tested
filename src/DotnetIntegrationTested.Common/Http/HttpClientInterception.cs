namespace DotnetIntegrationTested.Common.Http;

public static class HttpInterceptionExtensions
{
  public static void AddHttpClientInterceptionForTestsOnly(this IServiceCollection services)
  {
    services.AddSingleton<HttpInterception>();
    services.AddTransient<TestInterceptionHandler>();
  }
}

public sealed class HttpInterception
{
  public delegate Task<HttpResponseMessage> InterceptorFn(
    HttpRequestMessage request,
    CancellationToken cancellationToken
  );

  public Dictionary<string, InterceptorFn> Registry { get; } = new();
}

public sealed class TestInterceptionHandler : DelegatingHandler
{
  private readonly IServiceProvider _serviceProvider;

  public TestInterceptionHandler(IServiceProvider serviceProvider)
  {
    _serviceProvider = serviceProvider;
  }

  protected override async Task<HttpResponseMessage> SendAsync(
    HttpRequestMessage request,
    CancellationToken cancellationToken
  )
  {
    if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "IntegrationTests")
    {
      var interceptions = _serviceProvider.GetRequiredService<HttpInterception>();
      var interceptFn = interceptions
        .Registry.Single(x => request.RequestUri!.AbsolutePath.Contains(x.Key))
        .Value;

      return await interceptFn(request, cancellationToken);
    }

    return await base.SendAsync(request, cancellationToken);
  }
}
