namespace DotnetIntegrationTested.Common.Abstractions.Http;

public interface IEndpoint
{
  IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints);
}
