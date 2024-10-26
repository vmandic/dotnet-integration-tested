using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper.Contrib.Extensions;
using DotnetIntegrationTested.Common.Abstractions.Http;
using DotnetIntegrationTested.Services.Http;
using DotnetIntegrationTested.Services.Models.Sql;
using DotnetIntegrationTested.Services.SqlDb;
using DotnetIntegrationTested.Services.Tools;
using Microsoft.IdentityModel.Tokens;

namespace DotnetIntegrationTested.AuthApi.Endpoints.PostLogin;

public sealed class PostLoginEndpoint : IEndpoint
{
  public static string CreateJwt(IConfiguration config, string username, int userId)
  {
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtSecret =
      config["Jwt:Secret"]
      ?? throw new InvalidOperationException("Missing JWT secret config value");
    var jwtSecretKeyBytes = Encoding.UTF8.GetBytes(jwtSecret);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
      Subject = new ClaimsIdentity(
        new[]
        {
          new Claim(ClaimTypes.Name, username),
          new Claim(ClaimTypes.NameIdentifier, userId.ToString(), ClaimValueTypes.Integer),
        }
      ),
      Expires = DateTime.UtcNow.AddHours(1), // WARNING: Just don't do this, demo only
      SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(jwtSecretKeyBytes),
        SecurityAlgorithms.HmacSha256Signature
      ),
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
  }

  public IEndpointConventionBuilder Map(IEndpointRouteBuilder endpoints) =>
    endpoints.MapPost(
      "/login",
      async (
        PostLoginRequest request,
        SqlConnectionFactory db,
        RequestPayloadValidatorService validator,
        IConfiguration config,
        CancellationToken cancellationToken
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

        IEnumerable<User>? users;
        using (var conn = await db.CreateOpenConnectionAsync(cancellationToken))
        {
          // WARNING: A dreadful "repository", I won't even explain myself!
          users = await conn.GetAllAsync<User>();
        }

        var passwordHash = HashTools.GetMd5Hash(request.Password);
        var existingUser = users?.SingleOrDefault(x =>
          x.Username == request.Username && passwordHash == x.PasswordHash
        );

        if (existingUser is null)
        {
          return Results.Unauthorized();
        }

        var token = CreateJwt(config, request.Username, existingUser.Id);
        return Results.Ok(new PostLoginResponse(token));
      }
    );
}
