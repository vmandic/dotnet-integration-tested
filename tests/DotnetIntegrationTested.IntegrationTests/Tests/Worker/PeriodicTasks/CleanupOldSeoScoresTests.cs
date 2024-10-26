using DotnetIntegrationTested.Common.Abstractions.DateAndTime;
using DotnetIntegrationTested.IntegrationTests.Setup;
using DotnetIntegrationTested.Services.Models.MongoCollections;
using DotnetIntegrationTested.Services.MongoDb;
using DotnetIntegrationTested.Services.Tools;
using DotnetIntegrationTested.Worker.PeriodicTasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Xunit.Abstractions;

namespace DotnetIntegrationTested.IntegrationTests.Tests.Worker.PeriodicTasks;

public class CleanupOldSeoScoresTests : ParallelTestBase
{
  private readonly IServiceProvider _serviceProvider;

  public CleanupOldSeoScoresTests(
    ITestOutputHelper outputHelper,
    ParallelTestSuite parallelTestSuite
  )
    : base(outputHelper, parallelTestSuite, startOnlyMongo: true)
  {
    _serviceProvider = CreateDefaultServices().BuildServiceProvider();
  }

  [Fact]
  public async Task CleanupOldSeoScores_ShouldSucceed()
  {
    // Arrange
    var mongoDb = _serviceProvider.GetRequiredService<MongoDb>();
    var seoScores = mongoDb.GetCollection<SeoScore>();
    var dateTimeProvider =
      _serviceProvider.GetRequiredService<IDateTimeProvider>() as DateTimeProvider;
    var now = new DateTime(2024, 11, 1);
    dateTimeProvider!.SetNow(now);

    foreach (var i in Enumerable.Range(1, 10))
    {
      await seoScores.InsertOneAsync(
        new SeoScore
        {
          Id = ObjectId.GenerateNewId(dateTimeProvider.Now),
          UserId = i,
          Data = new
          {
            Keyword = Faker.Commerce.Product(),
            Url = Faker.Internet.UrlWithPath(),
            Score = Faker.Random.Int(1, 100),
          }.ToBsonDocument(),
        }
      );
    }

    var logger = _serviceProvider.GetRequiredService<ILogger<CleanupOldSeoScores>>();
    var worker = new CleanupOldSeoScores(logger, _serviceProvider);

    // Act & Assert:
    await worker.ProcessTaskAsync(default);

    var checkScores1 = await seoScores.AsQueryable().ToListAsync();
    checkScores1.Should().HaveCount(10);

    // Do time traveling, exactly 2mn + 1min
    dateTimeProvider.SetNow(now.AddMonths(2).AddMinutes(1));
    await worker.ProcessTaskAsync(default);

    var checkScores2 = await seoScores.AsQueryable().ToListAsync();
    checkScores2.Should().BeEmpty();
  }
}
