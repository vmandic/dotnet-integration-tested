using DotnetIntegrationTested.Services.Models.Sql;
using DotnetIntegrationTested.Services.Tools;
using FluentMigrator;

namespace DotnetIntegrationTested.SqlMigrations.Scripts
{
  [Migration(20241123_1652)]
  public class M_20241123_1652_AddTestUser : Migration
  {
    public override void Up()
    {
      Insert
        .IntoTable("users")
        .Row(new User { Username = "test1", PasswordHash = HashTools.GetMd5Hash("test") })
        .Row(new User { Username = "test2", PasswordHash = HashTools.GetMd5Hash("test") });
    }

    public override void Down() { }
  }
}
