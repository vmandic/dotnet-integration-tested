using System.Reflection;
using DotnetIntegrationTested.Common.Abstractions.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DotnetIntegrationTested.Common.Extensions.Http;

public static class EndpointExtensions
{
  public static IServiceCollection AddEndpoints(
    this IServiceCollection serviceCollection,
    Assembly executingAssembly
  )
  {
    var endpointType = typeof(IEndpoint);
    var descriptors = executingAssembly
      .DefinedTypes.Where(type => type.IsClass && type.ImplementedInterfaces.Contains(endpointType))
      .Select(type => ServiceDescriptor.Transient(endpointType, type))
      .ToList();

    serviceCollection.TryAddEnumerable(descriptors);

    return serviceCollection;
  }

  public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
  {
    var endpoints = builder.ServiceProvider.GetRequiredService<IEnumerable<IEndpoint>>();
    foreach (var endpoint in endpoints)
    {
      endpoint.Map(builder);
    }

    return builder;
  }
}
