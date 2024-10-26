using System.ComponentModel.DataAnnotations;
using DotnetIntegrationTested.HttpApi.Attributes;
using DotnetIntegrationTested.Services.Tools;

namespace DotnetIntegrationTested.HttpApi.Endpoints.SeoChecks.PostSeoCheck;

public sealed class PostSeoCheckRequest
{
  [Required]
  [Url(ErrorMessage = "Please enter a valid URL.")]
  public required string Url { get; set; }

  [Required]
  [MinLength(1)]
  [ValidKeywords]
  public required List<string> Keywords { get; set; }

  public string GetRedisKey() =>
    HashTools.GetMd5Hash($"seo-check-request:{Url}{string.Join(",", Keywords)}");
}
