using System.Net;
using System.Net.Http.Json;
using DotnetIntegrationTested.Common.Http;
using DotnetIntegrationTested.ExternalApis.Http.Wincher.Auth.Endpoints.PostLogin;
using DotnetIntegrationTested.ExternalApis.Http.Wincher.V1.Endpoints.PostOnPageSeoChecks;
using DotnetIntegrationTested.HttpApi.Endpoints.SeoChecks.PostSeoCheck;
using DotnetIntegrationTested.IntegrationTests.Extensions;
using DotnetIntegrationTested.IntegrationTests.Setup;
using DotnetIntegrationTested.Services.Models.MongoCollections;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Xunit.Abstractions;
using Xunit.Categories;

namespace DotnetIntegrationTested.IntegrationTests.Tests.HttpApi.Endpoints.SeoChecks.PostSeoCheck;

[Feature("SeoChecks")]
public sealed class PostSeoCheckEndpointTests : ParallelTestBase
{
  public PostSeoCheckEndpointTests(
    ITestOutputHelper outputHelper,
    ParallelTestSuite parallelTestSuite
  )
    : base(outputHelper, parallelTestSuite, startWorker: true) { }

  [Fact]
  public async Task PostSeoCheckEndpoint_ShouldSucceed()
  {
    // Arrange
    await AuthorizeHttpApiClientAsync();

    var keywordsCount = Faker.Random.Number(2, 5);
    var seoCheckRequest = new PostSeoCheckRequest
    {
      Url = Faker.Internet.UrlWithPath(),
      Keywords = Enumerable
        .Range(1, keywordsCount)
        .Select(_ => $"{Faker.Commerce.ProductAdjective()} {Faker.Commerce.ProductName()}")
        .ToList(),
    };

    var interceptors = HttpApiServiceProvider.GetRequiredService<HttpInterception>().Registry;
    interceptors[Configuration["WincherApi:Paths:PostOnPageSeoChecks"]!] = async (request, ct) =>
    {
      var requestPayload = await request.Content!.ReadFromJsonAsync<PostOnPageSeoChecksRequest>(
        cancellationToken: ct
      );

      return new HttpResponseMessage(HttpStatusCode.Created)
      {
        Content = new PostOnPageSeoChecksRequestResponse
        {
          Id = ObjectId.GenerateNewId().ToString(),
          Expired = false,
          Score = new OnPageSeoCheckScore
          {
            Score = Faker.Random.Int(1, 100),
            Url = requestPayload!.Url,
            Keyword = requestPayload.Keyword,
          },
        }.AsJsonHttpContent(),
      };
    };

    interceptors[Configuration["WincherAuth:Paths:PostLogin"]!] = (_, _) =>
      Task.FromResult(
        new HttpResponseMessage(HttpStatusCode.OK)
        {
          Content = new PostLoginResponse("fake").AsJsonHttpContent(),
        }
      );

    // Act
    var response = await HttpApiClient.PostAsJsonAsync("seo-check", seoCheckRequest);

    // Assert
    // Verify response
    response.EnsureSuccessStatusCode();

    // Verify RabbitMq consumer + MongoDb storage
    var mongoDb = GetMongoDb();
    var seoScoreCollection = mongoDb.GetCollection<SeoScore>();
    var ok = await WaitUntilSuccessAsync(async () =>
    {
      var seoScores = await seoScoreCollection.AsQueryable().ToListAsync();
      return seoScores.Count == keywordsCount;
    });

    ok.Should().BeTrue();

    // Verify Redis
    var isCached = await GetRedisDb()
      .GetJsonAsync<PostSeoCheckResponse>(seoCheckRequest.GetRedisKey());

    isCached.Should().NotBeNull();
  }
}
