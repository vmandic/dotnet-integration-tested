namespace DotnetIntegrationTested.Services.Configuration.Mq;

public sealed class RabbitMqConfig
{
  public required string Username { get; init; }

  public required string Password { get; init; }

  public required string HostAddress { get; init; }

  public required string VirtualHost { get; init; }

  public string[] HostAddresses =>
    HostAddress.Split(',')
    ?? throw new InvalidOperationException("Host address(es) cannot be empty");
}
