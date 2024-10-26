using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotnetIntegrationTested.Services.Models.MongoCollections;

public sealed class SeoScore
{
  [BsonId]
  [BsonElement("_id")]
  public ObjectId Id { get; init; }

  [BsonElement("user_id")]
  public int UserId { get; init; }

  [BsonElement("data")]
  public required BsonDocument Data { get; init; }
}
