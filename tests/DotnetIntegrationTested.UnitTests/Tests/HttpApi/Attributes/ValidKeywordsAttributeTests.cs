using System.ComponentModel.DataAnnotations;
using DotnetIntegrationTested.HttpApi.Attributes;

namespace DotnetIntegrationTested.UnitTests.Tests.HttpApi.Attributes;

public class ValidKeywordsAttributeTests
{
  [Theory]
  [InlineData("")] // Empty keyword
  [InlineData("   ")] // Whitespace-only keyword
  public void Validate_InvalidEmptyOrWhitespaceKeywords_ThrowsValidationException(string keyword)
  {
    // Arrange
    var attribute = new ValidKeywordsAttribute();
    var keywords = new List<string> { keyword };
    var validationContext = new ValidationContext(new object(), null, null);

    // Act & Assert
    var exception = Assert.Throws<ValidationException>(
      () => attribute.Validate(keywords, validationContext)
    );
    Assert.Equal(
      "Keywords cannot contain empty or whitespace entries.",
      exception.ValidationResult.ErrorMessage
    );
  }

  [Theory]
  [InlineData("invalid!")] // Contains invalid character
  [InlineData("invalid  keyword")] // Multiple spaces
  [InlineData("extra space ")] // Trailing space
  public void Validate_InvalidKeywords_ThrowsValidationException(string keyword)
  {
    // Arrange
    var attribute = new ValidKeywordsAttribute();
    var keywords = new List<string> { keyword };
    var validationContext = new ValidationContext(new object(), null, null);

    // Act & Assert
    var exception = Assert.Throws<ValidationException>(
      () => attribute.Validate(keywords, validationContext)
    );
    Assert.Equal(
      "Each keyword can only contain letters, numbers, hyphens and single white spaces.",
      exception.ValidationResult.ErrorMessage
    );
  }

  [Theory]
  [InlineData("valid-keyword")] // Simple valid keyword
  [InlineData("über-cool")] // Valid Unicode keyword
  [InlineData("another valid")] // Valid with single space
  public void Validate_ValidKeywords_ReturnsNoValidationException(string keyword)
  {
    // Arrange
    var attribute = new ValidKeywordsAttribute();
    var keywords = new List<string> { keyword };
    var validationContext = new ValidationContext(new object(), null, null);

    // Act
    attribute.Validate(keywords, validationContext);

    // Assert
    Assert.Equal(0, validationContext.Items.Count);
  }

  [Fact]
  public void Validate_KeywordLengthExceeds50_ThrowsValidationException()
  {
    // Arrange
    var attribute = new ValidKeywordsAttribute();
    var keywords = new List<string> { new string('a', 51) }; // 51-character keyword
    var validationContext = new ValidationContext(new object(), null, null);

    // Act & Assert
    var exception = Assert.Throws<ValidationException>(
      () => attribute.Validate(keywords, validationContext)
    );
    Assert.Equal(
      "Each keyword must be 50 characters or fewer.",
      exception.ValidationResult.ErrorMessage
    );
  }

  [Fact]
  public void Validate_NullOrNonListValue_ReturnsNoValidationException()
  {
    // Arrange
    var attribute = new ValidKeywordsAttribute();
    object? value = null;
    var validationContext = new ValidationContext(new object(), null, null);

    // Act
    attribute.Validate(value, validationContext);

    // Assert
    Assert.Equal(0, validationContext.Items.Count);
  }
}
