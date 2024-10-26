using System.Data;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace DotnetIntegrationTested.Services.SqlDb;

public sealed class SqlConnectionFactory
{
  private readonly string _connectionString;

  public SqlConnectionFactory(IConfiguration configuration)
  {
    _connectionString =
      configuration.GetConnectionString("Sql")
      ?? throw new InvalidOperationException("Missing connection string for SQL");
  }

  public async Task<IDbConnection> CreateOpenConnectionAsync(
    CancellationToken cancellationToken = default
  )
  {
    var conn = new MySqlConnection(_connectionString);
    await conn.OpenAsync(cancellationToken);

    return conn;
  }
}
