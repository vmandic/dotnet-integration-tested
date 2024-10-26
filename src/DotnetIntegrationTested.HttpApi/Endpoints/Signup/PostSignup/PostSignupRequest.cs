using System.ComponentModel.DataAnnotations;

namespace DotnetIntegrationTested.HttpApi.Endpoints.Signup.PostSignup;

public sealed record PostSignupRequest(
  [Required(AllowEmptyStrings = false)] string Username,
  [Required(AllowEmptyStrings = false)] string Password
);
