using FluentMigrator;

namespace DotnetIntegrationTested.SqlMigrations.Scripts
{
  [Migration(20241116_1910)]
  public class M_20241116_1910_InitSchema : Migration
  {
    public override void Up()
    {
      Execute.Sql(
        """
            CREATE TABLE users (
                Id INT AUTO_INCREMENT PRIMARY KEY,
                Username VARCHAR(100) UNIQUE NOT NULL,
                PasswordHash VARCHAR(255) NOT NULL, # WARNING I was super lazy to add a `salt`!
                CreatedAt DATETIME DEFAULT (UTC_TIMESTAMP())
            );

            CREATE UNIQUE INDEX uix_username ON users (Username);
        """
      );
    }

    public override void Down()
    {
      Execute.Sql("DROP TABLE users;");
    }
  }
}
