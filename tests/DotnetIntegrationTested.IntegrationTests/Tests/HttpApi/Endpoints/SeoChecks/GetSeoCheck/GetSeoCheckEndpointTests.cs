using System.Net.Http.Json;
using System.Text.Json;
using DotnetIntegrationTested.HttpApi.Endpoints.SeoChecks.GetSeoCheck;
using DotnetIntegrationTested.IntegrationTests.Setup;
using DotnetIntegrationTested.Services.Models.MongoCollections;
using FluentAssertions;
using MongoDB.Bson;
using Xunit.Abstractions;
using Xunit.Categories;

namespace DotnetIntegrationTested.IntegrationTests.Tests.HttpApi.Endpoints.SeoChecks.GetSeoCheck;

[Feature("SeoChecks")]
public class GetSeoCheckEndpointTests : ParallelTestBase
{
  public GetSeoCheckEndpointTests(
    ITestOutputHelper outputHelper,
    ParallelTestSuite parallelTestSuite
  )
    : base(outputHelper, parallelTestSuite) { }

  [Fact]
  public async Task GetSeoCheckEndpoint_ShouldSucceed()
  {
    // Arrange
    var mongoDb = GetMongoDb();
    var seoScoreCollection = mongoDb.GetCollection<SeoScore>();
    var seoScore = new SeoScore
    {
      Data = new
      {
        score = Faker.Random.Number(1, 100),
        keyword = Faker.Commerce.ProductName(),
        url = Faker.Internet.UrlWithPath(),
      }.ToBsonDocument(),
      Id = ObjectId.GenerateNewId(),
      UserId = 1,
    };
    await seoScoreCollection.InsertOneAsync(seoScore);
    await AuthorizeHttpApiClientAsync();

    // Act
    var response = await HttpApiClient.GetAsync($"seo-check/{seoScore.Id}");

    // Assert
    response.EnsureSuccessStatusCode();
    var model = await response.Content.ReadFromJsonAsync<GetSeoCheckResponse>();
    model.Should().NotBeNull();
    model!.Id.Should().Be(seoScore.Id.ToString());
    model.Data.Should().NotBeNull();
    var bsonDoc = BsonDocument.Parse(JsonSerializer.Serialize(model.Data));
    bsonDoc.Should().BeEquivalentTo(seoScore.Data);
  }
}
