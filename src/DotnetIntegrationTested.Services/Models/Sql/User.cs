using Dapper.Contrib.Extensions;

namespace DotnetIntegrationTested.Services.Models.Sql;

[Table("users")]
public sealed class User
{
  [Key]
  public int Id { get; set; }

  public required string Username { get; set; }

  public required string PasswordHash { get; set; }

  public DateTime CreatedAt { get; set; }
}
