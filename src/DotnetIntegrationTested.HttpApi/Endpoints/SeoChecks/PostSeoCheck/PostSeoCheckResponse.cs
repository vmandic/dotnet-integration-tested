using DotnetIntegrationTested.Services.Features.SeoChecker;

namespace DotnetIntegrationTested.HttpApi.Endpoints.SeoChecks.PostSeoCheck;

public sealed record PostSeoCheckResponse(
  string Url,
  IReadOnlyCollection<CheckKeywordsSeoScoreResult> Results
);
