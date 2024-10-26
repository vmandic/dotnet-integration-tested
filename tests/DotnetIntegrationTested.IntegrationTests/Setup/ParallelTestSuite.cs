using System.Diagnostics;

namespace DotnetIntegrationTested.IntegrationTests.Setup;

[CollectionDefinition(nameof(ParallelTestSuiteCollection))]
[EnableParallelization]
public sealed class ParallelTestSuiteCollection : ICollectionFixture<ParallelTestSuite> { }

public sealed class ParallelTestSuite : IAsyncLifetime
{
  private Stopwatch _sw = null!;

  /// <summary>
  /// InitializeAsync runs before all tests after <see cref="ParallelTestSuite"/> CTOR.
  /// </summary>
  /// <returns>Default async operation task.</returns>
  public Task InitializeAsync()
  {
    _sw = Stopwatch.StartNew();
    Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "IntegrationTests");
    Console.WriteLine($"Test suite execution start on: {DateTime.Now}");

    return Task.CompletedTask;
  }

  /// <summary>
  /// DisposeAsync runs after all tests.
  /// </summary>
  /// <returns>Default async operation task.</returns>
  public Task DisposeAsync()
  {
    Console.WriteLine($"Test suite execution done in: {_sw.Elapsed}");

    return Task.CompletedTask;
  }
}
