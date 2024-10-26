using DotnetIntegrationTested.Common.Abstractions.Http;
using Microsoft.AspNetCore.Authorization;

namespace DotnetIntegrationTested.HttpApi.Endpoints.Generic.GetCheckAuth;

public sealed class GetAuthCheckEndpoint : IEndpoint
{
  public IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
    endpoints.MapGet(
      "/check-auth",
      [Authorize]
      async (context) =>
      {
        var message = $"Authorized: {context.User.Identity?.Name}";
        context.Response.ContentType = "text/html";
        await context.Response.WriteAsync(message);
      }
    );
}
