using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DotnetIntegrationTested.Services.Http;

public sealed class RequestPayloadValidatorService
{
  private readonly IHttpContextAccessor _httpContextAccessor;

  public RequestPayloadValidatorService(IHttpContextAccessor httpContextAccessor)
  {
    _httpContextAccessor = httpContextAccessor;
  }

  public Dictionary<string, string[]>? Validate(object? requestPayload)
  {
    if (requestPayload is null)
    {
      return new()
      {
        { "Request payload", new[] { "Request payload is null or could not be bound" } },
      };
    }

    var httpContext =
      _httpContextAccessor.HttpContext
      ?? throw new InvalidOperationException("The required HttpContext is null");

    var validationResults = new List<ValidationResult>();
    var validationContext = new ValidationContext(
      requestPayload,
      serviceProvider: httpContext.RequestServices,
      items: null
    );

    if (
      !Validator.TryValidateObject(
        requestPayload,
        validationContext,
        validationResults,
        validateAllProperties: true
      )
    )
    {
      return validationResults
        .SelectMany(result =>
          result.MemberNames.Select(member => new { Member = member, result.ErrorMessage })
        )
        .GroupBy(x => x.Member)
        .ToDictionary(
          g => g.Key,
          g => g.Select(x => x.ErrorMessage ?? "Invalid value").Distinct().ToArray()
        );
    }

    return null;
  }
}
