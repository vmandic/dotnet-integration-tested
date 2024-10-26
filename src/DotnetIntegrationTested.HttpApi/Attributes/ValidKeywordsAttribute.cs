using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DotnetIntegrationTested.HttpApi.Attributes;

/// <inheritdoc />
public sealed partial class ValidKeywordsAttribute : ValidationAttribute
{
  protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
  {
    if (value is List<string> keywords)
    {
      foreach (var keyword in keywords)
      {
        if (string.IsNullOrWhiteSpace(keyword))
        {
          return new ValidationResult(
            "Keywords cannot contain empty or whitespace entries.",
            new[] { validationContext.MemberName }!
          );
        }

        if (keyword.Length > 50)
        {
          return new ValidationResult(
            "Each keyword must be 50 characters or fewer.",
            new[] { validationContext.MemberName }!
          );
        }

        if (!ValidKeywordRegex().IsMatch(keyword))
        {
          return new ValidationResult(
            "Each keyword can only contain letters, numbers, hyphens and single white spaces.",
            new[] { validationContext.MemberName }!
          );
        }
      }
    }

    return ValidationResult.Success!;
  }

  [GeneratedRegex(@"^[\p{L}\p{N}-'’]+( [\p{L}\p{N}-'’]+)*$", RegexOptions.Compiled)]
  private static partial Regex ValidKeywordRegex();
}
