using System.Collections.Concurrent;
using System.Net;
using DotnetIntegrationTested.ExternalApis.Http.Wincher.V1;

namespace DotnetIntegrationTested.Services.Features.SeoChecker;

public sealed class SeoCheckerService
{
  private readonly WincherHttpClientV1 _apiClientV1;

  public SeoCheckerService(WincherHttpClientV1 apiClientV1)
  {
    _apiClientV1 = apiClientV1;
  }

  public async Task<(
    List<CheckKeywordsSeoScoreResult> Results,
    HashSet<int> FailedCodes
  )> CheckKeywordsSeoScoreAsync(
    string url,
    IReadOnlyCollection<string> keywords,
    CancellationToken ct
  )
  {
    var failedCodes = new ConcurrentBag<int>();
    var keywordResults = new ConcurrentBag<CheckKeywordsSeoScoreResult>();

    await Parallel.ForEachAsync(
      keywords,
      new ParallelOptions { MaxDegreeOfParallelism = 2, CancellationToken = ct },
      async (keyword, keywordCt) =>
      {
        var (result, statusCode) = await _apiClientV1.PostOnPageSeoChecksAsync(
          new(url, keyword),
          keywordCt
        );
        if (statusCode == HttpStatusCode.Created && result is not null)
        {
          keywordResults.Add(
            new(
              keyword.ToLowerInvariant(),
              CheckOk: true,
              result.Id,
              result.Score?.Score ?? 0,
              result.Score?.DateAndTime ?? DateTime.UtcNow,
              result.Score
            )
          );
        }
        else
        {
          keywordResults.Add(new(keyword, CheckOk: false));
          failedCodes.Add((int)statusCode);
        }
      }
    );

    return (keywordResults.ToList(), failedCodes.ToHashSet());
  }
}
