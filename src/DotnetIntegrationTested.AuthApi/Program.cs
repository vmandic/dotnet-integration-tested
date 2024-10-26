using DotnetIntegrationTested.Services.Http;

namespace DotnetIntegrationTested.AuthApi;

public static class Program
{
  public static void Main(string[] args)
  {
    Console.Title = "AuthApi";
    WebHostTools.CreateDefaultBuilder<Startup>(args).Build().Run();
  }
}
