using System.Data;
using System.Diagnostics;
using Bogus;
using Dapper.Contrib.Extensions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using DotnetIntegrationTested.AuthApi.Endpoints.PostLogin;
using DotnetIntegrationTested.Common.Abstractions.DateAndTime;
using DotnetIntegrationTested.IntegrationTests.Extensions;
using DotnetIntegrationTested.Services.Extensions.Configuration;
using DotnetIntegrationTested.Services.Extensions.Mq;
using DotnetIntegrationTested.Services.Http;
using DotnetIntegrationTested.Services.Models.Sql;
using DotnetIntegrationTested.Services.MongoDb;
using DotnetIntegrationTested.Services.RedisDb;
using DotnetIntegrationTested.Services.SqlDb;
using DotnetIntegrationTested.Services.Tools;
using DotnetIntegrationTested.SqlMigrations;
using IdentityModel.Client;
using Meziantou.Extensions.Logging.Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Testcontainers.MariaDb;
using Testcontainers.MongoDb;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using Xunit.Abstractions;
using ConfigurationExtensions = DotnetIntegrationTested.Services.Extensions.Configuration.ConfigurationExtensions;

namespace DotnetIntegrationTested.IntegrationTests.Setup;

public sealed class TestContainerConfOverrides : Dictionary<string, string> { }

[Collection(nameof(ParallelTestSuiteCollection))]
[EnableParallelization]
public abstract class ParallelTestBase : IAsyncLifetime
{
  private readonly bool _startWorker;
  private readonly bool _startOnlyMongo;
  private readonly Stopwatch _sw;
  private readonly List<DockerContainer> _dockerContainers = new();
  private INetwork _network = null!;
  private MariaDbContainer? _mariaDbContainer;
  private MongoDbContainer _mongoDbContainer = null!;
  private RabbitMqContainer? _rabbitMqContainer;
  private RedisContainer _redisContainer = null!;
  private IHost? _bgWorkerService;

  protected IConfiguration Configuration { get; private set; } = null!;

  protected IServiceProvider HttpApiServiceProvider { get; private set; } = null!;

  protected HttpClient AuthApiClient { get; private set; } = null!;

  protected HttpClient HttpApiClient { get; private set; } = null!;

  protected TestServer HttpApiServer { get; private set; } = null!;

  protected TestServer AuthApiServer { get; private set; } = null!;

  protected ILogger Logger { get; }

  protected ITestOutputHelper OutputHelper { get; }

  protected ParallelTestSuite ParallelTestSuite { get; }

  protected string CurrentTestCaseName { get; }

  protected string CurrentTestId { get; }

  protected Faker Faker { get; } = new();

  /// <summary>
  /// InitializeAsync runs before each test startup after <see cref="ParallelTestBase"/> CTOR.
  /// </summary>
  /// <returns>Default async operation task.</returns>
  public async Task InitializeAsync()
  {
    var confOverrides = await CreateAndInitDbStackAsync();
    CreateApplicationServices(confOverrides);
  }

  /// <summary>
  /// DisposeAsync runs after each test.
  /// </summary>
  /// <returns>Default async operation task.</returns>
  public async Task DisposeAsync()
  {
    if (_bgWorkerService is not null)
    {
      await _bgWorkerService.StopAsync();
    }

    await Parallel.ForEachAsync(
      _dockerContainers,
      async (container, token) => await container.StopAsync(token)
    );

    await _network.DisposeAsync();

    Console.WriteLine(
      $"Finished test case: {CurrentTestCaseName}, ID: {CurrentTestId} @ {_sw.Elapsed}"
    );
  }

  private static IWebHostBuilder CreateTestWebHostBuilder<TStartup>(
    TestContainerConfOverrides confOverrides
  )
    where TStartup : class =>
    new WebHostBuilder()
      .UseEnvironment("IntegrationTests")
      .AddConfiguration(confOverrides)
      .UseStartup<TStartup>();

  protected ParallelTestBase(
    ITestOutputHelper outputHelper,
    ParallelTestSuite parallelTestSuite,
    bool startWorker = false,
    bool startOnlyMongo = false
  )
  {
    _startWorker = startWorker;

    // NOTE: this is just an VERY BASIC example of an optimization that you can apply
    _startOnlyMongo = startOnlyMongo;

    _sw = Stopwatch.StartNew();
    OutputHelper = outputHelper;
    ParallelTestSuite = parallelTestSuite;
    CurrentTestCaseName = OutputHelper.GetCurrentTestCaseName();
    CurrentTestId = GenerateTestId();
    Logger = new XUnitLoggerProvider(
      OutputHelper,
      new XUnitLoggerOptions { IncludeCategory = true }
    ).CreateLogger(CurrentTestId);

    Logger.LogInformation(
      "Start test case: {CurrentTestCaseName} at {Now}",
      CurrentTestCaseName,
      DateTime.Now
    );
  }

  protected MongoDb GetMongoDb() => HttpApiServiceProvider.GetRequiredService<MongoDb>();

  protected RedisDb GetRedisDb() => HttpApiServiceProvider.GetRequiredService<RedisDb>();

  protected Task<IDbConnection> GetOpenSqlConnectionAsync(CancellationToken ct = default)
  {
    var connFactory = HttpApiServiceProvider.GetRequiredService<SqlConnectionFactory>();
    return connFactory.CreateOpenConnectionAsync(ct);
  }

  protected async Task AuthorizeHttpApiClientAsync(int userId = 1)
  {
    var sqlDb = HttpApiServiceProvider.GetRequiredService<SqlConnectionFactory>();
    using var conn = await sqlDb.CreateOpenConnectionAsync();
    var user = await conn.GetAsync<User>(userId);

    if (user is null)
    {
      throw new InvalidOperationException($"Login user not found for ID: {userId}");
    }

    var jwtRaw = PostLoginEndpoint.CreateJwt(Configuration, user.Username, userId);
    HttpApiClient.SetBearerToken(jwtRaw);
  }

  protected IServiceCollection CreateDefaultServices()
  {
    var services = new ServiceCollection();
    services.AddLogging();
    services.AddTransient<IConfiguration>(_ => Configuration);
    services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    services.AddSingleton<MongoDb>();

    if (!_startOnlyMongo)
    {
      services.AddTransient<SqlConnectionFactory>();
      services.AddScoped<RedisDb>();
      services.AddMassTransitWithRabbitMq();
    }

    return services;
  }

  private void CreateApplicationServices(TestContainerConfOverrides confOverrides)
  {
    if (_startOnlyMongo)
    {
      var builder = new ConfigurationBuilder();
      ConfigurationExtensions.ConfigureConfigurationBuilder(
        "IntegrationTests",
        builder,
        confOverrides
      );

      Configuration = builder.Build();
      return;
    }

    var httpApiHostBuilder = CreateTestWebHostBuilder<HttpApi.Startup>(confOverrides)
      .ConfigureTestServices(services =>
      {
        // NOTE: not really needed for our requirements here, but if you have an IdP service
        // then your main APIs need to backchannel with IdP services and this is a part of the
        // solution, the other part is about consuming this service in your APIs, see Startup.cs
        var handler = AuthApiServer.CreateHandler();
        var handlerWrapper = new AuthApiServerHandlerWrapper(handler);
        services.AddSingleton(handlerWrapper);

        // NOTE: a good place to add additional stuff
      });

    var authApiHostBuilder = CreateTestWebHostBuilder<AuthApi.Startup>(confOverrides);

    if (_startWorker)
    {
      var bgWorkerHostBuilder = Worker.Startup.CreateDefaultBuilder(confOverrides: confOverrides);
      bgWorkerHostBuilder.Environment.EnvironmentName = "IntegrationTests";

      _bgWorkerService = bgWorkerHostBuilder.Build();
      _bgWorkerService.StartAsync();
    }

    AuthApiServer = new TestServer(authApiHostBuilder);
    HttpApiServer = new TestServer(httpApiHostBuilder);
    HttpApiClient = HttpApiServer.CreateClient();
    AuthApiClient = AuthApiServer.CreateClient();
    HttpApiServiceProvider = HttpApiServer.Services;
    Configuration = HttpApiServiceProvider.GetRequiredService<IConfiguration>();
  }

  private async Task<TestContainerConfOverrides> CreateAndInitDbStackAsync()
  {
    _network = new NetworkBuilder().Build();

    var mariaDbCnfPath = Path.GetFullPath("./Setup/docker/mariadb");
    if (!Directory.Exists(mariaDbCnfPath))
    {
      throw new FileNotFoundException("Required mariadb conf dir not found");
    }

    var mongoDbCnfPath = Path.GetFullPath("./Setup/docker/mongodb");
    if (!Directory.Exists(mongoDbCnfPath))
    {
      throw new FileNotFoundException("Required mongodb conf dir not found");
    }

    // NOTE: Testcontainers is just one of the ways to facilitate infrastructure, another
    // common approach would be (re)using docker-compose.yml (via a library:
    // https://github.com/mariotoffia/FluentDocker) and another way to manage resources more
    // effectively would be to reuse containers across tests but create new DBs for each test.
    var sqlDbMigrated = false;
    var mariaDbContainerTask = !_startOnlyMongo
      ? Task.Run(
        () =>
          new MariaDbBuilder()
            .WithName($"it_{CurrentTestId}_mariadb")
            .WithNetworkAliases(nameof(_mariaDbContainer))
            .DependsOn(_network)
            .WithDatabase("it_db")
            .WithUsername("root")
            .WithPassword("root")
            .WithBindMount(mariaDbCnfPath, "/etc/mysql/conf.d")
            .WithWaitStrategy(
              Wait.ForUnixContainer()
                .UntilPortIsAvailable(3306)
                .UntilMessageIsLogged("ready for connections")
            )
            .Build()
      )
      : Task.FromResult<MariaDbContainer>(null!);

    // NOTE: to match init.js, I mean, you could alter it on the fly here, the file...
    var mongoDatabaseName = "dotnet_integration_tested";
    var mongoDbContainerTask = Task.Run(
      () =>
        new MongoDbBuilder()
          .WithName($"it_{CurrentTestId}_mongodb")
          .WithNetworkAliases(nameof(_mongoDbContainer))
          .DependsOn(_network)
          .WithEnvironment("MONGO_INITDB_DATABASE", mongoDatabaseName)
          .WithUsername("root")
          .WithPassword("root")
          .WithBindMount(mongoDbCnfPath, "/docker-entrypoint-initdb.d")
          .WithWaitStrategy(
            Wait.ForUnixContainer()
              .UntilPortIsAvailable(27017) // MongoDB default port
              .UntilMessageIsLogged("Waiting for connections")
          )
          .Build()
    );

    var rabbitMqContainerTask = !_startOnlyMongo
      ? Task.Run(
        () =>
          new RabbitMqBuilder()
            .WithName($"it_{CurrentTestId}_rabbitmq")
            .WithNetworkAliases(nameof(_rabbitMqContainer))
            .DependsOn(_network)
            .WithUsername("guest")
            .WithPassword("guest")
            .WithEnvironment("RABBITMQ_DEFAULT_VHOST", "it_db")
            .WithWaitStrategy(
              Wait.ForUnixContainer().UntilMessageIsLogged("Server startup complete")
            )
            .Build()
      )
      : Task.FromResult<RabbitMqContainer>(null!);

    _mariaDbContainer = await mariaDbContainerTask;
    if (_mariaDbContainer is not null)
    {
      _mariaDbContainer.Started += (_, _) =>
      {
        var sw = Stopwatch.StartNew();
        var mariaDbConnectionString = _mariaDbContainer.GetConnectionString();
        var conn = new MySqlConnection(mariaDbConnectionString);

        // sanity test the connection
        conn.Open();
        conn.Close();
        conn.Dispose();

        SqlMigrator.ApplyMigrations(mariaDbConnectionString);
        sqlDbMigrated = true;

        Logger.LogDebug("SQL database migrated @ {Elapsed}", sw.Elapsed);
      };
    }

    _mongoDbContainer = await mongoDbContainerTask;
    _rabbitMqContainer = await rabbitMqContainerTask;

    // We can't wrap this one in a task, we need it to build immediately
    _redisContainer = !_startOnlyMongo
      ? new RedisBuilder()
        .WithName($"it_{CurrentTestId}_redis")
        .WithNetworkAliases(nameof(_redisContainer))
        .WithWaitStrategy(
          Wait.ForUnixContainer().UntilMessageIsLogged("Ready to accept connections")
        )
        .DependsOn(_network)
        .DependsOn(_mariaDbContainer)
        .DependsOn(_mongoDbContainer)
        .DependsOn(_rabbitMqContainer)
        .Build()
      : null!;

    if (_mariaDbContainer is not null)
    {
      _dockerContainers.Add(_mariaDbContainer);
    }

    _dockerContainers.Add(_mongoDbContainer);

    if (_rabbitMqContainer is not null)
    {
      _dockerContainers.Add(_rabbitMqContainer);
    }

    if (_redisContainer is not null)
    {
      _dockerContainers.Add(_redisContainer);
    }

    // docker failsafe
    var cts = new CancellationTokenSource();
    if (
      int.TryParse(Environment.GetEnvironmentVariable("TESTCONTAINERS_TIMEOUT_MS"), out var timeout)
      || timeout < 1
    )
    {
      timeout = 90_000;
    }

    cts.CancelAfter(timeout);

    var confOverrides = new TestContainerConfOverrides();
    if (_startOnlyMongo)
    {
      await _mongoDbContainer.StartAsync(cts.Token);
    }
    else
    {
      await _redisContainer!.StartAsync(cts.Token);
      confOverrides["ConnectionStrings:Sql"] = _mariaDbContainer!.GetConnectionString();
      confOverrides["ConnectionStrings:Redis"] = _redisContainer!.GetConnectionString();
      confOverrides["ConnectionStrings:RabbitMq"] =
        _rabbitMqContainer!.GetConnectionString() + "it_db";

      if (!await WaitUntilSuccessAsync(() => sqlDbMigrated))
      {
        throw new InvalidOperationException("SQL database was not migrated");
      }
    }

    confOverrides["ConnectionStrings:Mongo"] = _mongoDbContainer.GetConnectionString();
    confOverrides["Mongo:DatabaseName"] = mongoDatabaseName;

    return confOverrides;
  }

  private string GenerateTestId() => $"{DateTime.Now:HHmmss}_{Faker.Random.AlphaNumeric(6)}";
}
