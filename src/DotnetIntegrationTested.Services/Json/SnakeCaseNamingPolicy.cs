using System.Text.Json;

namespace DotnetIntegrationTested.Services.Json;

public sealed class SnakeCaseNamingPolicy : JsonNamingPolicy
{
  public static SnakeCaseNamingPolicy Instance { get; } = new();

  public override string ConvertName(string name)
  {
    var snakeCase = new List<char>();

    for (var i = 0; i < name.Length; i++)
    {
      // Insert underscore before uppercase letters (except for the first letter)
      if (i > 0 && char.IsUpper(name[i]) && !char.IsUpper(name[i + 1]))
      {
        snakeCase.Add('_');
      }

      // Add current character to the snakeCase list
      snakeCase.Add(name[i]);
    }

    // Convert the snakeCase list to a string and make it lowercase
    return new string(snakeCase.ToArray()).ToLower();
  }
}
