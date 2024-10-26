using DotnetIntegrationTested.IntegrationTests.Setup;
using FluentAssertions;
using Xunit.Abstractions;
using Xunit.Categories;

namespace DotnetIntegrationTested.IntegrationTests.Tests;

[Category("Sanity")]
public class SanityTests : ParallelTestBase
{
  public SanityTests(ITestOutputHelper outputHelper, ParallelTestSuite parallelTestSuite)
    : base(outputHelper, parallelTestSuite) { }

  [Fact]
  public async Task HttpApi_CheckAuth_ShouldSucceed()
  {
    // Arrange
    await AuthorizeHttpApiClientAsync();

    // Act
    var response = await HttpApiClient.GetAsync("check-auth");
    response.EnsureSuccessStatusCode();

    // Assert
    var data = await response.Content.ReadAsStringAsync();
    data.Should().NotBeNullOrEmpty();
    data.Should().Be("Authorized: test1");
  }
}
