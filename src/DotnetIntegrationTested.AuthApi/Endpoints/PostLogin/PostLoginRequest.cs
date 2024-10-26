using System.ComponentModel.DataAnnotations;

namespace DotnetIntegrationTested.AuthApi.Endpoints.PostLogin;

public sealed record PostLoginRequest(
  [Required(AllowEmptyStrings = false)] string Username,
  [Required(AllowEmptyStrings = false)] string Password
);
