namespace DotnetIntegrationTested.Worker;

public sealed class DefaultWorker : BackgroundService
{
  private readonly ILogger<DefaultWorker> _logger;

  public DefaultWorker(ILogger<DefaultWorker> logger)
  {
    _logger = logger;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    while (!stoppingToken.IsCancellationRequested)
    {
      _logger.LogInformation(
        "DotnetIntegrationTested.Worker running at: {Time}",
        DateTimeOffset.Now
      );
      await Task.Delay(60_000, stoppingToken);
    }
  }
}
