using System.Security.Claims;
using DotnetIntegrationTested.Common.Abstractions.Http;
using DotnetIntegrationTested.Services.Features.SeoChecker;
using DotnetIntegrationTested.Services.Http;
using DotnetIntegrationTested.Services.Models.MqContracts;
using DotnetIntegrationTested.Services.RedisDb;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace DotnetIntegrationTested.HttpApi.Endpoints.SeoChecks.PostSeoCheck;

public sealed class PostSeoCheckEndpoint : IEndpoint
{
  public IEndpointConventionBuilder Map(IEndpointRouteBuilder app) =>
    app.MapPost(
        "/seo-check",
        async (
          PostSeoCheckRequest request,
          RequestPayloadValidatorService validator,
          SeoCheckerService service,
          HttpRequest httpRequest,
          IPublishEndpoint mq,
          RedisDb redisDb,
          ILogger<PostSeoCheckEndpoint> logger,
          CancellationToken ct
        ) =>
        {
          // validate request
          var validationResults = validator.Validate(request);
          if (validationResults is not null)
          {
            return Results.ValidationProblem(
              validationResults,
              statusCode: StatusCodes.Status422UnprocessableEntity
            );
          }

          var cached = await redisDb.GetJsonAsync<PostSeoCheckResponse>(request.GetRedisKey());
          if (cached is not null)
          {
            logger.LogInformation("Using cached response from Redis");
            return Results.Ok(cached);
          }

          // get SEO scores
          var (keywordResults, failedCodes) = await service.CheckKeywordsSeoScoreAsync(
            request.Url,
            request.Keywords,
            ct
          );

          // store SEO check for user if logged in
          if (httpRequest.HttpContext.User.Identity?.IsAuthenticated == true)
          {
            var userId = int.Parse(
              httpRequest.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!
            );

            var messages = keywordResults
              .Where(x => x.RawScoreData is not null)
              .Select(x => new InsertSeoScore(userId, x.RawScoreData!));

            await mq.PublishBatch(messages, ct);
          }

          // verify results
          if (keywordResults.All(x => x.CheckId is null))
          {
            return Results.Problem(
              new ProblemDetails
              {
                Status = StatusCodes.Status424FailedDependency,
                Title = "SEO check failed",
                Detail =
                  "The upstream SEO checker service has failed for attempted keywords with "
                  + $"status codes: HTTP {string.Join(",", failedCodes)}, please try again later.",
              }
            );
          }

          // issue response with SEO score data
          var responseModel = new PostSeoCheckResponse(request.Url, keywordResults);
          await redisDb.SetJsonAsync(request.GetRedisKey(), responseModel);

          return Results.Ok(responseModel);
        }
      )
      .WithName("PostSeoCheck")
      .WithTags("Seo")
      .Produces<PostSeoCheckResponse>()
      .Produces(StatusCodes.Status422UnprocessableEntity)
      .Produces(StatusCodes.Status424FailedDependency)
      .Produces(499);
}
