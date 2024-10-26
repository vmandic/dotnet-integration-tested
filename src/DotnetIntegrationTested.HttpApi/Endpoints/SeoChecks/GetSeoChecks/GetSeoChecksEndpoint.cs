using System.Security.Claims;
using DotnetIntegrationTested.Common.Abstractions.Http;
using DotnetIntegrationTested.Services.Models.MongoCollections;
using DotnetIntegrationTested.Services.MongoDb;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DotnetIntegrationTested.HttpApi.Endpoints.SeoChecks.GetSeoChecks;

public sealed class GetSeoChecksEndpoint : IEndpoint
{
  public IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
    endpoints.MapGet(
      "/seo-checks",
      [Authorize]
      async (MongoDb mongoDb, HttpRequest httpRequest) =>
      {
        var seoScoreCollection = mongoDb.GetCollection<SeoScore>();
        var userId = int.Parse(
          httpRequest.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var seoScoresId = await seoScoreCollection
          .AsQueryable()
          .Where(x => x.UserId == userId)
          .OrderByDescending(x => x.Id)
          .Take(5)
          .Select(x => new
          {
            x.Id,
            Url = x.Data["Url"].ToString(),
            Keyword = x.Data["Keyword"].ToString(),
            Score = x.Data["Score"].ToString(),
          })
          .ToListAsync();

        return Results.Ok(
          seoScoresId.Select(x => new
          {
            Id = x.Id.ToString(),
            x.Id.CreationTime,
            x.Url,
            x.Keyword,
            Score = int.Parse(x.Score!),
          })
        );
      }
    );
}
