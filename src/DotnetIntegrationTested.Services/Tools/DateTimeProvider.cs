using DotnetIntegrationTested.Common.Abstractions.DateAndTime;

namespace DotnetIntegrationTested.Services.Tools;

public sealed class DateTimeProvider : IDateTimeProvider
{
  private TimeSpan _offset = TimeSpan.Zero;

  private DateTime InstanceValue => DateTime.Now.Add(_offset);

  private DateTime InstanceUtcValue => InstanceValue.ToUniversalTime();

  public DateTimeProvider(TimeSpan? offset = null)
  {
    if (offset.HasValue)
    {
      _offset = offset.Value;
    }
  }

  public DateTime Now => InstanceValue;

  public DateTime UtcNow => InstanceUtcValue;

  // public void SetOffset(TimeSpan offset) => _offset = offset;
  // public void AddOffset(TimeSpan offset) => _offset += offset;
  public void SetNow(DateTime startFrom)
  {
    _offset = startFrom - DateTime.Now;
  }
}
