using System.Text.Json.Serialization;
using DotnetIntegrationTested.ExternalApis.Http.Wincher.V1.Endpoints.PostOnPageSeoChecks;

namespace DotnetIntegrationTested.Services.Features.SeoChecker;

public sealed record CheckKeywordsSeoScoreResult(
  string Keyword,
  bool CheckOk,
  string? CheckId = default,
  int? Score = default,
  DateTime? CreatedAt = default,
  [property: JsonIgnore] OnPageSeoCheckScore? RawScoreData = null
);
