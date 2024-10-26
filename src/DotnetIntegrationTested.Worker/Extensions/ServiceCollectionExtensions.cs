namespace DotnetIntegrationTested.Worker.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddHostedServices(this IServiceCollection services)
  {
    // Find all types that implement BackgroundService in the given assembly
    var backgroundServices = typeof(ServiceCollectionExtensions)
      .Assembly.GetTypes()
      .Where(type => typeof(BackgroundService).IsAssignableFrom(type) && !type.IsAbstract)
      .ToList();

    // Register each BackgroundService as a hosted service
    foreach (var backgroundService in backgroundServices)
    {
      // Manually register the BackgroundService as IHostedService
      services.AddTransient(typeof(IHostedService), backgroundService);
    }

    return services;
  }
}
