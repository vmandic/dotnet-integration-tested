using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetIntegrationTested.SqlMigrations;

public static class SqlMigrator
{
  public static void ApplyMigrations(string connectionString)
  {
    var services = new ServiceCollection()
      .AddFluentMigratorCore()
      .ConfigureRunner(rb =>
        rb.AddMySql8()
          .WithGlobalConnectionString(connectionString)
          .ScanIn(typeof(SqlMigrator).Assembly)
          .For.Migrations()
      );

    // NOTE: enable in tests for debugging issues if required
    var isIntegrationTests =
      Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == "IntegrationTests";
    if (!isIntegrationTests)
    {
      services.AddLogging(lb => lb.AddFluentMigratorConsole());
    }

    var serviceProvider = services.BuildServiceProvider(false);

    var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
    if (!isIntegrationTests)
    {
      runner.ListMigrations();
    }

    runner.MigrateUp();
  }
}
