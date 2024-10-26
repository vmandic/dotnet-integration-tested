using System.Net.Http.Json;
using Dapper;
using Dapper.Contrib.Extensions;
using DotnetIntegrationTested.AuthApi.Endpoints.PostLogin;
using DotnetIntegrationTested.HttpApi.Endpoints.Signup.PostSignup;
using DotnetIntegrationTested.IntegrationTests.Setup;
using DotnetIntegrationTested.Services.Models.Sql;
using FluentAssertions;
using IdentityModel.Client;
using Xunit.Abstractions;
using Xunit.Categories;

namespace DotnetIntegrationTested.IntegrationTests.Tests.HttpApi.Scenarios;

[Feature("Signup")]
[Category("Scenario")]
public class UserSignupScenarioTests : ParallelTestBase
{
  public UserSignupScenarioTests(
    ITestOutputHelper outputHelper,
    ParallelTestSuite parallelTestSuite
  )
    : base(outputHelper, parallelTestSuite) { }

  [Fact]
  public async Task NewUser_Should_Signup_And_Login_Successfully()
  {
    // Arrange
    var signupRequest = new PostSignupRequest(Faker.Internet.UserName(), Faker.Internet.Password());

    // Act & Assert
    // 1. Signup
    var postSignupResponse = await HttpApiClient.PostAsJsonAsync("signup", signupRequest);
    postSignupResponse.EnsureSuccessStatusCode();

    using (var dbConn = await GetOpenSqlConnectionAsync())
    {
      var users = (await dbConn.GetAllAsync<User>()).AsList();
      users.Should().NotBeEmpty();
      users.Should().Contain(x => x.Username == signupRequest.Username);
    }

    // 2. Login
    var loginRequest = new PostLoginRequest(signupRequest.Username, signupRequest.Password);
    var postLoginResponse = await AuthApiClient.PostAsJsonAsync("login", loginRequest);
    postLoginResponse.EnsureSuccessStatusCode();

    var tokenResponse = await postLoginResponse.Content.ReadFromJsonAsync<PostLoginResponse>();
    tokenResponse.Should().NotBeNull();
    tokenResponse!.Token.Should().NotBeNullOrEmpty();

    // 3. Verify with authorized endpoint
    HttpApiClient.SetBearerToken(tokenResponse.Token);
    var checkAuthResponse = await HttpApiClient.GetAsync("check-auth");
    checkAuthResponse.EnsureSuccessStatusCode();
    var data = await checkAuthResponse.Content.ReadAsStringAsync();
    data.Should().NotBeNullOrEmpty();
    data.Should().Be($"Authorized: {signupRequest.Username}");
  }
}
