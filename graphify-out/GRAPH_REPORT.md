# Graph Report - dotnet-integration-tested  (2026-06-10)

## Corpus Check
- 95 files · ~9,952 words
- Verdict: corpus is large enough that graph structure adds value.

## Summary
- 730 nodes · 757 edges · 84 communities (69 shown, 15 thin omitted)
- Extraction: 99% EXTRACTED · 1% INFERRED · 0% AMBIGUOUS · INFERRED: 10 edges (avg confidence: 0.8)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `1c51b9c8`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- [[_COMMUNITY_Community 0|Community 0]]
- [[_COMMUNITY_Community 1|Community 1]]
- [[_COMMUNITY_Community 2|Community 2]]
- [[_COMMUNITY_Community 3|Community 3]]
- [[_COMMUNITY_Community 4|Community 4]]
- [[_COMMUNITY_Community 5|Community 5]]
- [[_COMMUNITY_Community 6|Community 6]]
- [[_COMMUNITY_Community 7|Community 7]]
- [[_COMMUNITY_Community 8|Community 8]]
- [[_COMMUNITY_Community 9|Community 9]]
- [[_COMMUNITY_Community 10|Community 10]]
- [[_COMMUNITY_Community 11|Community 11]]
- [[_COMMUNITY_Community 12|Community 12]]
- [[_COMMUNITY_Community 13|Community 13]]
- [[_COMMUNITY_Community 14|Community 14]]
- [[_COMMUNITY_Community 15|Community 15]]
- [[_COMMUNITY_Community 16|Community 16]]
- [[_COMMUNITY_Community 17|Community 17]]
- [[_COMMUNITY_Community 18|Community 18]]
- [[_COMMUNITY_Community 19|Community 19]]
- [[_COMMUNITY_Community 20|Community 20]]
- [[_COMMUNITY_Community 21|Community 21]]
- [[_COMMUNITY_Community 22|Community 22]]
- [[_COMMUNITY_Community 23|Community 23]]
- [[_COMMUNITY_Community 24|Community 24]]
- [[_COMMUNITY_Community 25|Community 25]]
- [[_COMMUNITY_Community 26|Community 26]]
- [[_COMMUNITY_Community 27|Community 27]]
- [[_COMMUNITY_Community 28|Community 28]]
- [[_COMMUNITY_Community 29|Community 29]]
- [[_COMMUNITY_Community 30|Community 30]]
- [[_COMMUNITY_Community 31|Community 31]]
- [[_COMMUNITY_Community 32|Community 32]]
- [[_COMMUNITY_Community 33|Community 33]]
- [[_COMMUNITY_Community 34|Community 34]]
- [[_COMMUNITY_Community 35|Community 35]]
- [[_COMMUNITY_Community 36|Community 36]]
- [[_COMMUNITY_Community 37|Community 37]]
- [[_COMMUNITY_Community 38|Community 38]]
- [[_COMMUNITY_Community 39|Community 39]]
- [[_COMMUNITY_Community 40|Community 40]]
- [[_COMMUNITY_Community 41|Community 41]]
- [[_COMMUNITY_Community 42|Community 42]]
- [[_COMMUNITY_Community 43|Community 43]]
- [[_COMMUNITY_Community 44|Community 44]]
- [[_COMMUNITY_Community 45|Community 45]]
- [[_COMMUNITY_Community 46|Community 46]]
- [[_COMMUNITY_Community 47|Community 47]]
- [[_COMMUNITY_Community 48|Community 48]]
- [[_COMMUNITY_Community 49|Community 49]]
- [[_COMMUNITY_Community 50|Community 50]]
- [[_COMMUNITY_Community 51|Community 51]]
- [[_COMMUNITY_Community 52|Community 52]]
- [[_COMMUNITY_Community 53|Community 53]]
- [[_COMMUNITY_Community 54|Community 54]]
- [[_COMMUNITY_Community 55|Community 55]]
- [[_COMMUNITY_Community 56|Community 56]]
- [[_COMMUNITY_Community 57|Community 57]]
- [[_COMMUNITY_Community 58|Community 58]]
- [[_COMMUNITY_Community 60|Community 60]]
- [[_COMMUNITY_Community 61|Community 61]]
- [[_COMMUNITY_Community 62|Community 62]]
- [[_COMMUNITY_Community 63|Community 63]]
- [[_COMMUNITY_Community 64|Community 64]]
- [[_COMMUNITY_Community 65|Community 65]]
- [[_COMMUNITY_Community 66|Community 66]]
- [[_COMMUNITY_Community 67|Community 67]]
- [[_COMMUNITY_Community 68|Community 68]]
- [[_COMMUNITY_Community 69|Community 69]]
- [[_COMMUNITY_Community 70|Community 70]]

## God Nodes (most connected - your core abstractions)
1. `ParallelTestBase` - 27 edges
2. `RedisDb` - 10 edges
3. `scripts` - 8 edges
4. `IEndpoint` - 8 edges
5. `WincherHttpClientV1` - 8 edges
6. `WincherAuth` - 8 edges
7. `https` - 7 edges
8. `Task` - 7 edges
9. `RunSummary` - 7 edges
10. `ParallelTestClassRunner` - 7 edges

## Surprising Connections (you probably didn't know these)
- `GetSeoCheckEndpointTests` --inherits--> `ParallelTestBase`  [EXTRACTED]
  tests/DotnetIntegrationTested.IntegrationTests/Tests/HttpApi/Endpoints/SeoChecks/GetSeoCheck/GetSeoCheckEndpointTests.cs → tests/DotnetIntegrationTested.IntegrationTests/Setup/ParallelTestBase.cs
- `CleanupOldSeoScoresTests` --inherits--> `ParallelTestBase`  [EXTRACTED]
  tests/DotnetIntegrationTested.IntegrationTests/Tests/Worker/PeriodicTasks/CleanupOldSeoScoresTests.cs → tests/DotnetIntegrationTested.IntegrationTests/Setup/ParallelTestBase.cs
- `PostSeoCheckEndpointTests` --inherits--> `ParallelTestBase`  [EXTRACTED]
  tests/DotnetIntegrationTested.IntegrationTests/Tests/HttpApi/Endpoints/SeoChecks/PostSeoCheck/PostSeoCheckEndpointTests.cs → tests/DotnetIntegrationTested.IntegrationTests/Setup/ParallelTestBase.cs
- `UserSignupScenarioTests` --inherits--> `ParallelTestBase`  [EXTRACTED]
  tests/DotnetIntegrationTested.IntegrationTests/Tests/HttpApi/Scenarios/UserSignupScenarioTests.cs → tests/DotnetIntegrationTested.IntegrationTests/Setup/ParallelTestBase.cs
- `SanityTests` --inherits--> `ParallelTestBase`  [EXTRACTED]
  tests/DotnetIntegrationTested.IntegrationTests/Tests/SanityTests.cs → tests/DotnetIntegrationTested.IntegrationTests/Setup/ParallelTestBase.cs

## Import Cycles
- None detected.

## Communities (84 total, 15 thin omitted)

### Community 0 - "Community 0"
Cohesion: 0.07
Nodes (34): AssemblyName, Attribute, CancellationTokenSource, IEnumerable, IMessageBus, IMessageSink, int, IReflectionMethodInfo (+26 more)

### Community 1 - "Community 1"
Cohesion: 0.05
Nodes (39): Bogus (35.6.1), FluentAssertions (6.12.2), FluentMigrator (6.2.0), FluentMigrator.Runner.MySql (6.2.0), IdentityModel (7.0.0), Meziantou.Extensions.Logging.Xunit (1.0.6), Microsoft.AspNetCore.TestHost (7.0.20), Nito.AsyncEx.Tasks (5.1.2) (+31 more)

### Community 2 - "Community 2"
Cohesion: 0.06
Nodes (22): GetAuthCheckEndpoint, GetSeoCheckEndpoint, GetSeoChecksEndpoint, IEndpoint, PostLoginEndpoint, PostSeoCheckEndpoint, PostSignupEndpoint, IConfiguration (+14 more)

### Community 3 - "Community 3"
Cohesion: 0.06
Nodes (32): AllowedHosts, ConnectionStrings, Mongo, Redis, Sql, Jwt, Secret, Logging (+24 more)

### Community 4 - "Community 4"
Cohesion: 0.12
Nodes (17): AccessToken, AsyncLock, HttpResonseCode, HttpResponseCode, HttpStatusCode, PostOnPageSeoChecksRequest, PostOnPageSeoChecksRequestResponse, Result (+9 more)

### Community 5 - "Community 5"
Cohesion: 0.10
Nodes (19): AllowedHosts, ConnectionStrings, Mongo, Redis, Sql, Jwt, Secret, Logging (+11 more)

### Community 6 - "Community 6"
Cohesion: 0.10
Nodes (19): AllowedHosts, ConnectionStrings, Mongo, Redis, Sql, Jwt, Secret, Logging (+11 more)

### Community 7 - "Community 7"
Cohesion: 0.12
Nodes (7): Migration, DotnetIntegrationTested.SqlMigrations.Scripts, M_20241116_1909_InitMigrations, DotnetIntegrationTested.SqlMigrations.Scripts, M_20241116_1910_InitSchema, DotnetIntegrationTested.SqlMigrations.Scripts, M_20241123_1652_AddTestUser

### Community 8 - "Community 8"
Cohesion: 0.13
Nodes (15): launchBrowser, launchUrl, ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl (+7 more)

### Community 9 - "Community 9"
Cohesion: 0.16
Nodes (10): BackgroundService, DefaultWorker, CleanupOldSeoScores, CancellationToken, ILogger, Task, CancellationToken, ILogger (+2 more)

### Community 10 - "Community 10"
Cohesion: 0.13
Nodes (14): ConnectionStrings, Mongo, Sql, Logging, LogLevel, Default, Microsoft.Hosting.Lifetime, Mongo (+6 more)

### Community 11 - "Community 11"
Cohesion: 0.16
Nodes (9): ConnectionMultiplexer, IAsyncDisposable, IDatabase, IDisposable, RedisDb, IJsonSerializer, T, Task (+1 more)

### Community 12 - "Community 12"
Cohesion: 0.14
Nodes (13): author, description, name, repository, scripts, docker:build, format, lint (+5 more)

### Community 13 - "Community 13"
Cohesion: 0.15
Nodes (13): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+5 more)

### Community 14 - "Community 14"
Cohesion: 0.15
Nodes (11): bool, IHost, INetwork, MariaDbContainer, MongoDbContainer, RabbitMqContainer, RedisContainer, ParallelTestBase (+3 more)

### Community 15 - "Community 15"
Cohesion: 0.15
Nodes (10): DelegatingHandler, HttpInterception, HttpInterceptionExtensions, TestInterceptionHandler, HttpRequestMessage, HttpResponseMessage, CancellationToken, IServiceCollection (+2 more)

### Community 16 - "Community 16"
Cohesion: 0.15
Nodes (12): MassTransit.RabbitMQ (8.1.2), MongoDB.Driver (3.0.0), net7.0, CSharpier.MsBuild (0.29.2), Dapper.Contrib (2.0.78), MassTransit (8.1.2), Microsoft.Extensions.DependencyInjection (7.0.0), Microsoft.Extensions.Logging.Abstractions (8.0.2) (+4 more)

### Community 17 - "Community 17"
Cohesion: 0.17
Nodes (10): CheckKeywordsSeoScoreResult, FailedCodes, HashSet, IReadOnlyCollection, Results, SeoCheckerService, CancellationToken, List (+2 more)

### Community 18 - "Community 18"
Cohesion: 0.17
Nodes (11): Microsoft.AspNetCore.Authentication.JwtBearer (7.0.20), net7.0, CSharpier.MsBuild (0.29.2), Dapper (2.1.35), Dapper.Contrib (2.0.78), MassTransit (8.1.2), Microsoft.Extensions.DependencyInjection (7.0.0), Microsoft.Extensions.Logging.Abstractions (8.0.2) (+3 more)

### Community 19 - "Community 19"
Cohesion: 0.17
Nodes (11): net7.0, coverlet.collector (3.2.0), CSharpier.MsBuild (0.29.2), Microsoft.Extensions.Logging.Abstractions (8.0.2), Microsoft.NET.Test.Sdk (17.5.0), NSubstitute (5.3.0), StyleCop.Analyzers (1.2.0-beta.556), xunit (2.4.2) (+3 more)

### Community 20 - "Community 20"
Cohesion: 0.21
Nodes (8): HttpContext, ExceptionMiddleware, ExceptionMiddlewareExtensions, RequestDelegate, Exception, IApplicationBuilder, ILogger, Task

### Community 21 - "Community 21"
Cohesion: 0.18
Nodes (10): Microsoft.Extensions.Configuration.Abstractions (7.0.0), Microsoft.Extensions.Configuration.Binder (7.0.4), Microsoft.Extensions.Http (7.0.0), Nito.AsyncEx (5.1.2), net7.0, CSharpier.MsBuild (0.29.2), Microsoft.Extensions.DependencyInjection (7.0.0), Microsoft.Extensions.Logging.Abstractions (8.0.2) (+2 more)

### Community 22 - "Community 22"
Cohesion: 0.24
Nodes (6): Action, IBusRegistrationConfigurator, IRabbitMqBusFactoryConfigurator, MassTransitExtensions, RabbitMqConfig, IServiceCollection

### Community 23 - "Community 23"
Cohesion: 0.31
Nodes (4): ValidKeywordsAttributeTests, InlineData, Fact, Theory

### Community 24 - "Community 24"
Cohesion: 0.20
Nodes (9): AllowedHosts, ConnectionStrings, Sql, Jwt, Secret, Logging, LogLevel, Default (+1 more)

### Community 25 - "Community 25"
Cohesion: 0.22
Nodes (4): Func, Task, Task, GlobalTools

### Community 26 - "Community 26"
Cohesion: 0.22
Nodes (7): IAsyncLifetime, ICollectionFixture, ParallelTestSuite, ParallelTestSuite, ParallelTestSuiteCollection, Stopwatch, Task

### Community 27 - "Community 27"
Cohesion: 0.20
Nodes (6): IJsonSerializer, CustomJsonSerializer, CustomJsonSerializerExtensions, IServiceCollection, JsonSerializerOptions, T

### Community 28 - "Community 28"
Cohesion: 0.25
Nodes (6): ValidKeywordsAttribute, GeneratedRegex, Regex, ValidationAttribute, ValidationContext, ValidationResult

### Community 29 - "Community 29"
Cohesion: 0.28
Nodes (7): ConsumeContext, InsertSeoScoreConsumer, IConsumer, InsertSeoScore, ILogger, MongoDb, Task

### Community 30 - "Community 30"
Cohesion: 0.22
Nodes (8): CanonicalInstructions, IndexingInstructions, OnPageSeoCheckCategory, OnPageSeoCheckRating, OnPageSeoCheckScore, PostOnPageSeoChecksRequestResponse, ReducedOnPageSeoCheckScore, RobotInstructions

### Community 31 - "Community 31"
Cohesion: 0.25
Nodes (6): AuthApiServerHandlerWrapper, Startup, IApplicationBuilder, IConfiguration, IServiceCollection, IServiceProvider

### Community 32 - "Community 32"
Cohesion: 0.32
Nodes (5): ConfigurationExtensions, IConfigurationBuilder, Dictionary, HostApplicationBuilder, IWebHostBuilder

### Community 33 - "Community 33"
Cohesion: 0.25
Nodes (7): net7.0, CSharpier.MsBuild (0.29.2), Microsoft.Extensions.DependencyInjection.Abstractions (7.0.0), Microsoft.Extensions.Logging.Abstractions (7.0.1), MySqlConnector (2.2.7), StyleCop.Analyzers (1.2.0-beta.556), Microsoft.NET.Sdk.Web

### Community 34 - "Community 34"
Cohesion: 0.25
Nodes (4): PostSeoCheckEndpointTests, RedisDb, Fact, Task

### Community 35 - "Community 35"
Cohesion: 0.25
Nodes (7): Microsoft.Extensions.Hosting (7.0.1), Microsoft.NET.Sdk.Worker, net7.0, CSharpier.MsBuild (0.29.2), MassTransit (8.1.2), Microsoft.Extensions.Logging.Abstractions (8.0.2), StyleCop.Analyzers (1.2.0-beta.556)

### Community 36 - "Community 36"
Cohesion: 0.29
Nodes (5): IMongoCollection, IMongoDatabase, MongoDb, IConfiguration, T

### Community 37 - "Community 37"
Cohesion: 0.25
Nodes (7): dotnet-integration-tested, How to build, How to test, How to use, Lecture summary, Prerequirements, Project and lecture goal

### Community 38 - "Community 38"
Cohesion: 0.25
Nodes (5): UserSignupScenarioTests, CancellationToken, IDbConnection, Fact, Task

### Community 39 - "Community 39"
Cohesion: 0.29
Nodes (4): Assembly, EndpointExtensions, IEndpointRouteBuilder, IServiceCollection

### Community 40 - "Community 40"
Cohesion: 0.29
Nodes (6): commands, version, isRoot, tools, csharpier, version

### Community 41 - "Community 41"
Cohesion: 0.29
Nodes (4): IDateTimeProvider, DateTime, TimeSpan, DateTimeProvider

### Community 42 - "Community 42"
Cohesion: 0.29
Nodes (4): GetSeoCheckEndpointTests, MongoDb, Fact, Task

### Community 43 - "Community 43"
Cohesion: 0.33
Nodes (4): DotnetIntegrationTested.UnitTests.Tests.Services.Http, InvalidRequestPayload, RequestPayloadValidatorServiceTests, Fact

### Community 44 - "Community 44"
Cohesion: 0.29
Nodes (6): commandName, dotnetRunMessages, environmentVariables, DOTNET_ENVIRONMENT, DotnetIntegrationTested.Worker, profiles

### Community 45 - "Community 45"
Cohesion: 0.29
Nodes (5): SqlConnectionFactory, CancellationToken, IDbConnection, string, Task

### Community 46 - "Community 46"
Cohesion: 0.33
Nodes (3): Startup, IApplicationBuilder, IServiceCollection

### Community 47 - "Community 47"
Cohesion: 0.33
Nodes (4): CleanupOldSeoScoresTests, Fact, IServiceProvider, Task

### Community 48 - "Community 48"
Cohesion: 0.33
Nodes (3): Fact, Task, SanityTests

### Community 49 - "Community 49"
Cohesion: 0.40
Nodes (3): Dictionary, TestContainerConfOverrides, IWebHostBuilder

### Community 50 - "Community 50"
Cohesion: 0.40
Nodes (4): diagnosticMessages, maxParallelThreads, $schema, stopOnFail

### Community 51 - "Community 51"
Cohesion: 0.40
Nodes (3): Startup, Dictionary, HostApplicationBuilder

### Community 52 - "Community 52"
Cohesion: 0.40
Nodes (3): ObjectExtensions, StringContent, JsonSerializerOptions

### Community 53 - "Community 53"
Cohesion: 0.40
Nodes (3): RequestPayloadValidatorService, IHttpContextAccessor, Dictionary

### Community 54 - "Community 54"
Cohesion: 0.40
Nodes (3): WebHostTools, Dictionary, IWebHostBuilder

## Knowledge Gaps
- **367 isolated node(s):** `version`, `isRoot`, `version`, `commands`, `version` (+362 more)
  These have ≤1 connection - possible missing edges or undocumented components.
- **15 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `ParallelTestBase` connect `Community 14` to `Community 34`, `Community 38`, `Community 42`, `Community 47`, `Community 48`, `Community 49`, `Community 25`, `Community 26`?**
  _High betweenness centrality (0.018) - this node is a cross-community bridge._
- **Why does `RedisDb` connect `Community 34` to `Community 49`, `Community 2`?**
  _High betweenness centrality (0.015) - this node is a cross-community bridge._
- **What connects `version`, `isRoot`, `version` to the rest of the system?**
  _367 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Community 0` be split into smaller, more focused modules?**
  _Cohesion score 0.07474747474747474 - nodes in this community are weakly interconnected._
- **Should `Community 1` be split into smaller, more focused modules?**
  _Cohesion score 0.047619047619047616 - nodes in this community are weakly interconnected._
- **Should `Community 2` be split into smaller, more focused modules?**
  _Cohesion score 0.05555555555555555 - nodes in this community are weakly interconnected._
- **Should `Community 3` be split into smaller, more focused modules?**
  _Cohesion score 0.06060606060606061 - nodes in this community are weakly interconnected._