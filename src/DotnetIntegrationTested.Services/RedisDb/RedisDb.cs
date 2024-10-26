using DotnetIntegrationTested.Common.Abstractions.Json;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace DotnetIntegrationTested.Services.RedisDb;

public sealed class RedisDb : IDisposable, IAsyncDisposable
{
  private readonly IJsonSerializer _jsonSerializer;
  private readonly ConnectionMultiplexer _redisConnection;
  private readonly IDatabase _database;

  public RedisDb(IJsonSerializer jsonSerializer, IConfiguration config)
  {
    _jsonSerializer = jsonSerializer;
    _redisConnection = ConnectionMultiplexer.Connect(config.GetConnectionString("Redis")!);
    _database = _redisConnection.GetDatabase();
  }

  // Store an object as JSON in Redis
  public async Task<bool> SetJsonAsync<T>(string key, T value)
    where T : notnull
  {
    string jsonValue = _jsonSerializer.Serialize(value);
    return await _database.StringSetAsync(key, jsonValue, TimeSpan.FromHours(1));
  }

  // Retrieve a JSON object from Redis and deserialize it
  public async Task<T?> GetJsonAsync<T>(string key)
  {
    string? jsonValue = await _database.StringGetAsync(key);
    return string.IsNullOrEmpty(jsonValue) ? default : _jsonSerializer.Deserialize<T>(jsonValue);
  }

  public async ValueTask DisposeAsync()
  {
    await _redisConnection.DisposeAsync();
  }

  public void Dispose()
  {
    _redisConnection.Dispose();
  }
}
