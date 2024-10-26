namespace DotnetIntegrationTested.IntegrationTests.Tools;

public static class GlobalTools
{
  public static async Task<bool> WaitUntilSuccessAsync(
    Func<bool> action,
    int retries = 10,
    int delayMs = 1000
  )
  {
    bool isSuccess;
    do
    {
      await Task.Delay(delayMs);
      isSuccess = action();
    } while (retries-- > 0 && !isSuccess);

    return isSuccess;
  }

  public static async Task<bool> WaitUntilSuccessAsync(
    Func<Task<bool>> asyncFn,
    int retries = 10,
    int delayMs = 1000
  )
  {
    bool isSuccess;
    do
    {
      await Task.Delay(delayMs);
      isSuccess = await asyncFn();
    } while (retries-- > 0 && !isSuccess);

    return isSuccess;
  }
}
