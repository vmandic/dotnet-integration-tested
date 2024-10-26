using System.Diagnostics;
using DotnetIntegrationTested.Common.Abstractions.DateAndTime;
using DotnetIntegrationTested.Services.Models.MongoCollections;
using DotnetIntegrationTested.Services.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DotnetIntegrationTested.Worker.PeriodicTasks;

public sealed class CleanupOldSeoScores : BackgroundService
{
  private readonly ILogger<CleanupOldSeoScores> _logger;
  private readonly IServiceProvider _serviceProvider;

  public CleanupOldSeoScores(ILogger<CleanupOldSeoScores> logger, IServiceProvider serviceProvider)
  {
    _logger = logger;
    _serviceProvider = serviceProvider;
  }

  internal async Task ProcessTaskAsync(CancellationToken stoppingToken)
  {
    await using var scope = _serviceProvider.CreateAsyncScope();
    var mongoDb = scope.ServiceProvider.GetRequiredService<MongoDb>();
    var seoScores = mongoDb.GetCollection<SeoScore>();
    var dateTimeProvider = scope.ServiceProvider.GetRequiredService<IDateTimeProvider>();

    _logger.LogInformation("Starting delete of old SEO scores");
    var deleteUpToId = ObjectId.GenerateNewId(dateTimeProvider.Now.AddMonths(-2));
    var idsToDelete = await seoScores
      .AsQueryable()
      .Where(x => x.Id < deleteUpToId)
      .Select(x => x.Id)
      .ToListAsync(stoppingToken);

    _logger.LogInformation("Deleting {IdsCount} documents", idsToDelete.Count);
    var sw = Stopwatch.StartNew();

    var filterBuilder = Builders<SeoScore>.Filter.Where(x => idsToDelete.Contains(x.Id));
    var result = await seoScores.DeleteManyAsync(filterBuilder, stoppingToken);

    _logger.LogInformation(
      "Deleted {DeletedCount} @ {Elapsed}, next run is in 1 hour",
      result.DeletedCount,
      sw.Elapsed
    );
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    _logger.LogInformation("Worker started");

    while (!stoppingToken.IsCancellationRequested)
    {
      await ProcessTaskAsync(stoppingToken);
      await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
    }

    _logger.LogInformation("Worker stopping");
  }
}
