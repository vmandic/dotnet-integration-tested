using DotnetIntegrationTested.Services.Http;

namespace DotnetIntegrationTested.HttpApi;

public static class Program
{
  public static void Main(string[] args)
  {
    Console.Title = "HttpApi";
    WebHostTools.CreateDefaultBuilder<Startup>(args).Build().Run();
  }
}
