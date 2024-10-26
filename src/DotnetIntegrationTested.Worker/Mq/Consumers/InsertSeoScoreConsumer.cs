using DotnetIntegrationTested.Services.Models.MongoCollections;
using DotnetIntegrationTested.Services.Models.MqContracts;
using DotnetIntegrationTested.Services.MongoDb;
using MassTransit;
using MongoDB.Bson;

namespace DotnetIntegrationTested.Worker.Mq.Consumers;

public sealed class InsertSeoScoreConsumer : IConsumer<InsertSeoScore>
{
  private readonly ILogger<InsertSeoScoreConsumer> _logger;
  private readonly MongoDb _mongoDb;

  public InsertSeoScoreConsumer(ILogger<InsertSeoScoreConsumer> logger, MongoDb mongoDb)
  {
    _logger = logger;
    _mongoDb = mongoDb;
  }

  public async Task Consume(ConsumeContext<InsertSeoScore> context)
  {
    var doc = new SeoScore
    {
      Id = ObjectId.GenerateNewId(),
      UserId = context.Message.UserId,
      Data = context.Message.SeoScoreData.ToBsonDocument(),
    };

    await _mongoDb
      .GetCollection<SeoScore>()
      .InsertOneAsync(doc, cancellationToken: context.CancellationToken);

    _logger.LogInformation(
      "SEO score inserted for keyword: '{Keyword}', URL: '{Url}' and user ID: {UserId}",
      context.Message.SeoScoreData.Keyword,
      context.Message.SeoScoreData.Url,
      context.Message.UserId
    );
  }
}
