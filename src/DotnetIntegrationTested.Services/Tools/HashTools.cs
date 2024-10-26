using System.Security.Cryptography;
using System.Text;

namespace DotnetIntegrationTested.Services.Tools;

public static class HashTools
{
  // WARNING: just don't use for secure things like I do for this demo, use BCrypt e.g.
  public static string GetMd5Hash(string input) =>
    Convert.ToBase64String(MD5.HashData(Encoding.UTF8.GetBytes(input)));
}
