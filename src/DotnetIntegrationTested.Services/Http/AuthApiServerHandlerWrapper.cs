namespace DotnetIntegrationTested.Services.Http;

public sealed class AuthApiServerHandlerWrapper
{
  public HttpMessageHandler Handler { get; }

  public AuthApiServerHandlerWrapper(HttpMessageHandler handler)
  {
    Handler = handler;
  }
}
