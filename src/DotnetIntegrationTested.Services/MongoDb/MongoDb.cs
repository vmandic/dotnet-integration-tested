using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace DotnetIntegrationTested.Services.MongoDb;

public class MongoDb
{
  private readonly IConfiguration _config;
  private IMongoDatabase? _db;

  public MongoDb(IConfiguration config)
  {
    _config = config;
  }

  public IMongoCollection<T> GetCollection<T>() => Db.GetCollection<T>(typeof(T).Name);

  private IMongoDatabase Db => _db ??= GetDatabase();

  private IMongoDatabase GetDatabase()
  {
    var mongoConnectionUrl = new MongoUrl(_config.GetConnectionString("Mongo"));
    var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);
    return new MongoClient(mongoClientSettings).GetDatabase(_config["Mongo:DatabaseName"]!);
  }
}
