namespace DotnetIntegrationTested.Common.Abstractions.DateAndTime;

public interface IDateTimeProvider
{
  DateTime Now { get; }

  DateTime UtcNow { get; }
}
