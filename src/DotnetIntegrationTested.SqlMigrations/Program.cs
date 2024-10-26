using DotnetIntegrationTested.SqlMigrations;

// like, this is just for demo, get it ;-)
const string connectionString =
  "server=localhost;user=root;password=root;database=dotnet_integration_tested;port=3306;";

SqlMigrator.ApplyMigrations(connectionString);

Console.WriteLine("\nSQL migrations run finished.");
