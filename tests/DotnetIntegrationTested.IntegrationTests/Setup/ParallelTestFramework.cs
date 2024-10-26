using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

[assembly: TestFramework(
  "DotnetIntegrationTested.IntegrationTests.Setup.ParallelTestFramework",
  "DotnetIntegrationTested.IntegrationTests"
)]

namespace DotnetIntegrationTested.IntegrationTests.Setup;

/// <summary>
/// <see cref="ParallelTestFramework"/> is a custom Xunit test framework which extends the basic
/// behavior by allowing parallelization of test classes, methods and cases.
/// </summary>
/// <remarks>
/// Inspiration taken from the following links.
/// <ul>
///   <li>https://github.com/meziantou/Meziantou.Xunit.ParallelTestFramework</li>
///   <li>https://andrewlock.net/tracking-down-a-hanging-xunit-test-in-ci-building-a-custom-test-framework</li>
/// </ul>
/// </remarks>
public sealed class ParallelTestFramework : XunitTestFramework
{
  // NOTE: Be careful with this, realistically you do NOT WANT to run all tests at once as
  // docker containers are resource hogs, this lets you control how many concurrent classes
  // you can run at once, note that this combines with the "maxParallelThreads" configuration.
  // For example, having this 2 and maxParallelThreads: 2 can effectively make you run 8 tests
  // cases at once based on the current semaphore implementation of this framework, 8 test
  // cases means that you will be spinning up e.g. 32 docker containers at one moment
  internal const int TestClassesThreads = 2;

  public ParallelTestFramework(IMessageSink messageSink)
    : base(messageSink) { }

  protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName) =>
    new ParallelTestFrameworkExecutor(
      assemblyName,
      SourceInformationProvider,
      DiagnosticMessageSink
    );
}

public sealed class ParallelTestFrameworkExecutor : XunitTestFrameworkExecutor
{
  public ParallelTestFrameworkExecutor(
    AssemblyName assemblyName,
    ISourceInformationProvider sourceInformationProvider,
    IMessageSink diagnosticMessageSink
  )
    : base(assemblyName, sourceInformationProvider, diagnosticMessageSink) { }

  protected override async void RunTestCases(
    IEnumerable<IXunitTestCase> testCases,
    IMessageSink executionMessageSink,
    ITestFrameworkExecutionOptions executionOptions
  )
  {
    using var assemblyRunner = new ParallelTestAssemblyRunner(
      TestAssembly,
      testCases,
      DiagnosticMessageSink,
      executionMessageSink,
      executionOptions
    );

    await assemblyRunner.RunAsync().ConfigureAwait(false);
  }
}

public sealed class ParallelTestAssemblyRunner : XunitTestAssemblyRunner
{
  private readonly ITestFrameworkExecutionOptions _executionOptions;

  public ParallelTestAssemblyRunner(
    ITestAssembly testAssembly,
    IEnumerable<IXunitTestCase> testCases,
    IMessageSink diagnosticMessageSink,
    IMessageSink executionMessageSink,
    ITestFrameworkExecutionOptions executionOptions
  )
    : base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
  {
    _executionOptions = executionOptions;
  }

  protected override Task<RunSummary> RunTestCollectionAsync(
    IMessageBus messageBus,
    ITestCollection testCollection,
    IEnumerable<IXunitTestCase> testCases,
    CancellationTokenSource cancellationTokenSource
  )
  {
    return new ParallelTestCollectionRunner(
      testCollection,
      testCases,
      DiagnosticMessageSink,
      messageBus,
      TestCaseOrderer,
      new ExceptionAggregator(Aggregator),
      cancellationTokenSource,
      _executionOptions
    ).RunAsync();
  }
}

public sealed class ParallelTestCollectionRunner : XunitTestCollectionRunner
{
  private readonly SemaphoreSlim _testClassesSemaphore;
  private readonly ITestFrameworkExecutionOptions _executionOptions;

  public ParallelTestCollectionRunner(
    ITestCollection testCollection,
    IEnumerable<IXunitTestCase> testCases,
    IMessageSink diagnosticMessageSink,
    IMessageBus messageBus,
    ITestCaseOrderer testCaseOrderer,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    ITestFrameworkExecutionOptions executionOptions
  )
    : base(
      testCollection,
      testCases,
      diagnosticMessageSink,
      messageBus,
      testCaseOrderer,
      aggregator,
      cancellationTokenSource
    )
  {
    _executionOptions = executionOptions;
    _testClassesSemaphore = new(ParallelTestFramework.TestClassesThreads);
  }

  protected override async Task<RunSummary> RunTestClassAsync(
    ITestClass testClass,
    IReflectionTypeInfo @class,
    IEnumerable<IXunitTestCase> testCases
  )
  {
    await _testClassesSemaphore.WaitAsync(CancellationTokenSource.Token).ConfigureAwait(false);

    try
    {
      return await new ParallelTestClassRunner(
        testClass,
        @class,
        testCases,
        DiagnosticMessageSink,
        MessageBus,
        TestCaseOrderer,
        new ExceptionAggregator(Aggregator),
        CancellationTokenSource,
        CollectionFixtureMappings,
        _executionOptions
      ).RunAsync();
    }
    finally
    {
      _testClassesSemaphore.Release();
    }
  }

  protected override async Task<RunSummary> RunTestClassesAsync()
  {
    if (TestCollection.CollectionDefinition != null)
    {
      var enableParallelizationAttribute = TestCollection
        .CollectionDefinition.GetCustomAttributes(typeof(EnableParallelizationAttribute))
        .Any();
      if (enableParallelizationAttribute)
      {
        var summary = new RunSummary();

        var classTasks = TestCases
          .GroupBy(tc => tc.TestMethod.TestClass, TestClassComparer.Instance)
          .Select(tc => RunTestClassAsync(tc.Key, (IReflectionTypeInfo)tc.Key.Class, tc));

        var classSummaries = await Task.WhenAll(classTasks)
#if !NETSTANDARD
          .WaitAsync(CancellationTokenSource.Token)
#endif
          .ConfigureAwait(false);
        foreach (var classSummary in classSummaries)
        {
          summary.Aggregate(classSummary);
        }

        return summary;
      }
    }

    // Fall back to default behavior
    return await base.RunTestClassesAsync().ConfigureAwait(false);
  }
}

public sealed class ParallelTestClassRunner : XunitTestClassRunner
{
  private readonly ITestFrameworkExecutionOptions _executionOptions;
  private readonly SemaphoreSlim _testMethodsSemaphore;

  public ParallelTestClassRunner(
    ITestClass testClass,
    IReflectionTypeInfo @class,
    IEnumerable<IXunitTestCase> testCases,
    IMessageSink diagnosticMessageSink,
    IMessageBus messageBus,
    ITestCaseOrderer testCaseOrderer,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    IDictionary<Type, object> collectionFixtureMappings,
    ITestFrameworkExecutionOptions executionOptions
  )
    : base(
      testClass,
      @class,
      testCases,
      diagnosticMessageSink,
      messageBus,
      testCaseOrderer,
      aggregator,
      cancellationTokenSource,
      collectionFixtureMappings
    )
  {
    _executionOptions = executionOptions;
    _testMethodsSemaphore = new(_executionOptions.MaxParallelThreadsOrDefault());
  }

  // This method has been slightly modified from the original implementation to run tests in parallel
  // https://github.com/xunit/xunit/blob/2.4.2/src/xunit.execution/Sdk/Frameworks/Runners/TestClassRunner.cs#L194-L219
  protected override async Task<RunSummary> RunTestMethodsAsync()
  {
    var disableParallelizationAttribute = TestClass
      .Class.GetCustomAttributes(typeof(DisableParallelizationAttribute))
      .Any();

    var disableParallelizationOnCustomCollection =
      TestClass.Class.GetCustomAttributes(typeof(CollectionAttribute)).Any()
      && !TestClass.Class.GetCustomAttributes(typeof(EnableParallelizationAttribute)).Any();

    var disableParallelization =
      disableParallelizationAttribute || disableParallelizationOnCustomCollection;

    if (disableParallelization)
    {
      return await base.RunTestMethodsAsync().ConfigureAwait(false);
    }

    var summary = new RunSummary();
    IEnumerable<IXunitTestCase> orderedTestCases;
    try
    {
      orderedTestCases = TestCaseOrderer.OrderTestCases(TestCases);
    }
    catch (Exception ex)
    {
      var innerEx = Unwrap(ex);
      DiagnosticMessageSink.OnMessage(
        new DiagnosticMessage(
          $"Test case orderer '{TestCaseOrderer.GetType().FullName}' threw "
            + $"'{innerEx.GetType().FullName}' during ordering: {innerEx.Message}"
            + $"{Environment.NewLine}{innerEx.StackTrace}"
        )
      );
      orderedTestCases = TestCases.ToList();
    }

    var constructorArguments = CreateTestClassConstructorArguments();
    var methodGroups = orderedTestCases.GroupBy(tc => tc.TestMethod, TestMethodComparer.Instance);

    var methodTasks = methodGroups.Select(m =>
      RunTestMethodAsync(m.Key, (IReflectionMethodInfo)m.Key.Method, m, constructorArguments)
    );

    var methodSummaries = await Task.WhenAll(methodTasks).ConfigureAwait(false);

    foreach (var methodSummary in methodSummaries)
    {
      summary.Aggregate(methodSummary);
    }

    return summary;
  }

  protected override async Task<RunSummary> RunTestMethodAsync(
    ITestMethod testMethod,
    IReflectionMethodInfo method,
    IEnumerable<IXunitTestCase> testCases,
    object[] constructorArguments
  )
  {
    await _testMethodsSemaphore.WaitAsync(CancellationTokenSource.Token).ConfigureAwait(false);

    try
    {
      var summary = await new ParallelTestMethodRunner(
        testMethod,
        Class,
        method,
        testCases,
        DiagnosticMessageSink,
        MessageBus,
        new ExceptionAggregator(Aggregator),
        CancellationTokenSource,
        constructorArguments,
        _executionOptions
      ).RunAsync();

      return summary;
    }
    finally
    {
      _testMethodsSemaphore.Release();
    }
  }

  private static Exception Unwrap(Exception ex)
  {
    while (true)
    {
      if (
        ex is not TargetInvocationException targetInvocationException
        || targetInvocationException.InnerException is null
      )
      {
        return ex;
      }

      ex = targetInvocationException.InnerException;
    }
  }
}

public sealed class ParallelTestMethodRunner : XunitTestMethodRunner
{
  private readonly object[] _constructorArguments;
  private readonly SemaphoreSlim _testCasesSemaphore;
  private readonly IMessageSink _diagnosticMessageSink;

  public ParallelTestMethodRunner(
    ITestMethod testMethod,
    IReflectionTypeInfo @class,
    IReflectionMethodInfo method,
    IEnumerable<IXunitTestCase> testCases,
    IMessageSink diagnosticMessageSink,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource,
    object[] constructorArguments,
    ITestFrameworkExecutionOptions executionOptions
  )
    : base(
      testMethod,
      @class,
      method,
      testCases,
      diagnosticMessageSink,
      messageBus,
      aggregator,
      cancellationTokenSource,
      constructorArguments
    )
  {
    _constructorArguments = constructorArguments;
    _testCasesSemaphore = new(executionOptions.MaxParallelThreadsOrDefault());
    _diagnosticMessageSink = diagnosticMessageSink;
  }

  // This method has been slightly modified from the original implementation to run tests in parallel
  // https://github.com/xunit/xunit/blob/2.4.2/src/xunit.execution/Sdk/Frameworks/Runners/TestMethodRunner.cs#L130-L142
  protected override async Task<RunSummary> RunTestCasesAsync()
  {
    var cts = new CancellationTokenSource();
    var cancelAfterTimeSpan = TimeSpan.FromMinutes(3);
    cts.CancelAfter(cancelAfterTimeSpan);

    var disableParallelization = TestMethod
      .TestClass.Class.GetCustomAttributes(typeof(DisableParallelizationAttribute))
      .Any();

    try
    {
      if (disableParallelization)
      {
        return await base.RunTestCasesAsync().WaitAsync(cts.Token).ConfigureAwait(false);
      }

      var summary = new RunSummary();

      var caseTasks = TestCases.Select(RunTestCaseAsync);
      var caseSummaries = await Task.WhenAll(caseTasks).WaitAsync(cts.Token).ConfigureAwait(false);

      foreach (var caseSummary in caseSummaries)
      {
        summary.Aggregate(caseSummary);
      }

      return summary;
    }
    catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException)
    {
      Console.WriteLine($"Test execution cancelled as it took more than: {cancelAfterTimeSpan}");
      throw;
    }
  }

  protected override async Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
  {
    await _testCasesSemaphore.WaitAsync(CancellationTokenSource.Token).ConfigureAwait(false);

    try
    {
      // Create a new TestOutputHelper for each test case since they cannot be reused when running in parallel
      var args = _constructorArguments
        .Select(a => a is TestOutputHelper ? new TestOutputHelper() : a)
        .ToArray();

      var action = () =>
        testCase.RunAsync(
          _diagnosticMessageSink,
          MessageBus,
          args,
          new ExceptionAggregator(Aggregator),
          CancellationTokenSource
        );

      // Respect MaxParallelThreads by using the MaxConcurrencySyncContext if it exists, mimicking how collections are run
      // https://github.com/xunit/xunit/blob/2.4.2/src/xunit.execution/Sdk/Frameworks/Runners/XunitTestAssemblyRunner.cs#L169-L176
      if (SynchronizationContext.Current != null)
      {
        var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        return await Task
          .Factory.StartNew(
            action,
            CancellationTokenSource.Token,
            TaskCreationOptions.DenyChildAttach | TaskCreationOptions.HideScheduler,
            scheduler
          )
          .Unwrap()
          .ConfigureAwait(false);
      }

      return await Task.Run(action, CancellationTokenSource.Token).ConfigureAwait(false);
    }
    finally
    {
      _testCasesSemaphore.Release();
    }
  }
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class EnableParallelizationAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class DisableParallelizationAttribute : Attribute { }
