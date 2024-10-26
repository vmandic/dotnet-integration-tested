using System.Reflection;
using Xunit.Abstractions;

namespace DotnetIntegrationTested.IntegrationTests.Extensions;

public static class TestOutputHelperExtensions
{
  public static string GetCurrentTestCaseName(this ITestOutputHelper outputHelper)
  {
    Type type = outputHelper.GetType();
    FieldInfo? fieldInfo = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);

    object? fieldValue = fieldInfo?.GetValue(outputHelper);
    if (fieldValue is ITest xUnitTest)
    {
      // ignores FQN, use just the method name
      return xUnitTest.DisplayName.Split(".").Last();
    }

    return "UNKNOWN_TEST_NAME";
  }
}
