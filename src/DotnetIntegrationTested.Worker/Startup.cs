using DotnetIntegrationTested.Common.Abstractions.DateAndTime;
using DotnetIntegrationTested.Services.Extensions.Configuration;
using DotnetIntegrationTested.Services.Extensions.Mq;
using DotnetIntegrationTested.Services.MongoDb;
using DotnetIntegrationTested.Services.Tools;
using DotnetIntegrationTested.Worker.Extensions;
using DotnetIntegrationTested.Worker.Mq.Consumers;
using MassTransit;

namespace DotnetIntegrationTested.Worker;

public static class Startup
{
  public static HostApplicationBuilder CreateDefaultBuilder(
    string[]? args = null,
    Dictionary<string, string>? confOverrides = null
  )
  {
    var hostApplicationBuilder = Host.CreateApplicationBuilder(args)
      .AddConfiguration(confOverrides);
    hostApplicationBuilder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    hostApplicationBuilder.Services.AddSingleton<MongoDb>();
    hostApplicationBuilder.Services.AddHostedServices();
    hostApplicationBuilder.Services.AddMassTransitWithRabbitMq(bus => // SUCH RESILIENCY, SUCH WOW!
      bus.AddConsumer<InsertSeoScoreConsumer>((_, cfg) => cfg.UseMessageRetry(r => r.Immediate(2)))
    );
    return hostApplicationBuilder;
  }
}
