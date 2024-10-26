using System.ComponentModel.DataAnnotations;
using DotnetIntegrationTested.Services.Http;
using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace DotnetIntegrationTested.UnitTests.Tests.Services.Http
{
  public class RequestPayloadValidatorServiceTests
  {
    [Fact]
    public void Validate_NullRequestPayload_ReturnsValidationError()
    {
      // Arrange
      var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
      var service = new RequestPayloadValidatorService(httpContextAccessor);

      // Act
      var result = service.Validate(null);

      // Assert
      Assert.NotNull(result);
      Assert.Single(result);
      Assert.Contains(result, kvp => kvp.Key == "Request payload");
    }

    [Fact]
    public void Validate_InvalidRequestPayload_ReturnsValidationErrors()
    {
      // Arrange
      var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
      var service = new RequestPayloadValidatorService(httpContextAccessor);
      var requestPayload = new InvalidRequestPayload();

      // Act
      var result = service.Validate(requestPayload);

      // Assert
      Assert.NotNull(result);
      Assert.Single(result);
      Assert.Contains(result, kvp => kvp.Key == "InvalidProperty");
    }

    private class InvalidRequestPayload
    {
      [Required]
      public string? InvalidProperty { get; set; }
    }
  }
}
