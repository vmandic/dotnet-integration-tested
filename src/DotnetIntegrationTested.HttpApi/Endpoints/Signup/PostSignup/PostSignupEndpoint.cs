using Dapper.Contrib.Extensions;
using DotnetIntegrationTested.Common.Abstractions.Http;
using DotnetIntegrationTested.Services.Http;
using DotnetIntegrationTested.Services.Models.Sql;
using DotnetIntegrationTested.Services.SqlDb;
using DotnetIntegrationTested.Services.Tools;

namespace DotnetIntegrationTested.HttpApi.Endpoints.Signup.PostSignup;

public sealed class PostSignupEndpoint : IEndpoint
{
  public IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
    endpoints.MapPost(
      "/signup",
      async (
        PostSignupRequest request,
        RequestPayloadValidatorService validator,
        SqlConnectionFactory sqlConnectionFactory
      ) =>
      {
        var validationResults = validator.Validate(request);
        if (validationResults is not null)
        {
          return Results.ValidationProblem(
            validationResults,
            statusCode: StatusCodes.Status422UnprocessableEntity
          );
        }

        var hashedPassword = HashTools.GetMd5Hash(request.Password);
        using var conn = await sqlConnectionFactory.CreateOpenConnectionAsync();

        var users = await conn.GetAllAsync<User>();
        if (
          users.Any(x =>
            string.Equals(x.Username, request.Username, StringComparison.InvariantCultureIgnoreCase)
          )
        )
        {
          return Results.Problem(detail: "Username already taken", statusCode: 400);
        }

        await conn.InsertAsync(
          new User
          {
            CreatedAt = DateTime.UtcNow,
            PasswordHash = hashedPassword,
            Username = request.Username,
          }
        );

        return Results.Ok();
      }
    );
}
