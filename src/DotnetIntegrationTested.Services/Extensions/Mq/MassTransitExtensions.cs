using DotnetIntegrationTested.Services.Configuration.Mq;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetIntegrationTested.Services.Extensions.Mq;

public static class MassTransitExtensions
{
  public static void AddMassTransitWithRabbitMq(
    this IServiceCollection serviceCollection,
    Action<IBusRegistrationConfigurator>? registerConsumersFn = null
  )
  {
    serviceCollection.AddMassTransit(bus =>
    {
      registerConsumersFn?.Invoke(bus);
      bus.UsingRabbitMq(
        (context, busConfigurator) =>
        {
          var configuration = context.GetRequiredService<IConfiguration>();
          var connectionString = configuration.GetConnectionString("RabbitMq");
          if (connectionString != null)
          {
            busConfigurator.Host(connectionString);
          }
          else
          {
            var rabbitMqConfig =
              configuration.GetRequiredSection("RabbitMq").Get<RabbitMqConfig>()
              ?? throw new InvalidOperationException("Missing RabbitMq config value");

            ConfigureRabbitMqHosts(rabbitMqConfig, busConfigurator);
          }

          busConfigurator.ConfigureEndpoints(context);
        }
      );
    });
  }

  private static void ConfigureRabbitMqHosts(
    RabbitMqConfig config,
    IRabbitMqBusFactoryConfigurator busConfigurator
  )
  {
    string hostAddress = GetRabbitMqHostUriAddress(
      config.Username,
      config.Password,
      config.HostAddresses.First(),
      config.VirtualHost
    );

    busConfigurator.Host(hostAddress);
  }

  private static string GetRabbitMqHostUriAddress(
    string? username,
    string? password,
    string hostAddress,
    string? virtualHost
  ) => $"amqp://{username}:{password}@{hostAddress}/{virtualHost}";
}
