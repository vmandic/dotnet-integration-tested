using System.Security.Claims;
using DotnetIntegrationTested.Common.Abstractions.Http;
using DotnetIntegrationTested.Services.Models.MongoCollections;
using DotnetIntegrationTested.Services.MongoDb;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DotnetIntegrationTested.HttpApi.Endpoints.SeoChecks.GetSeoCheck;

public sealed class GetSeoCheckEndpoint : IEndpoint
{
  public IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
    endpoints.MapGet(
      "/seo-check/{id:required:minlength(24)}",
      [Authorize]
      async (
        string id,
        ILogger<GetSeoCheckEndpoint> logger,
        MongoDb mongoDb,
        HttpRequest httpRequest
      ) =>
      {
        var seoScoreCollection = mongoDb.GetCollection<SeoScore>();
        var userId = int.Parse(
          httpRequest.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        try
        {
          var seoScore = await seoScoreCollection
            .AsQueryable()
            .SingleOrDefaultAsync(x => x.Id == new ObjectId(id) && x.UserId == userId);

          if (seoScore is null)
          {
            return Results.NotFound();
          }

          var seoScoreJson = BsonTypeMapper.MapToDotNetValue(seoScore.Data);
          return Results.Ok(
            new GetSeoCheckResponse(seoScore.Id.ToString(), seoScore.Id.CreationTime, seoScoreJson)
          );
        }
        catch (Exception e)
        {
          // e.g. bad Id passed etc.
          logger.LogError(e, e.Message);

          // Duh, the server is never wrong ;-), but return HTTP 500
          return Results.Problem(detail: "Something went sideways...");
        }
      }
    );
}
