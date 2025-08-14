// ITimeService.cs
public interface ITimeService
{
    DateTime UtcNow { get; }
    DateTime VietnamNow { get; }

    DateTime ToUtc(DateTime local, string tzId = "SE Asia Standard Time");
    DateTimeOffset ToUtc(DateTimeOffset local);

    DateTime ToVietnamTime(DateTime utc);
    DateTimeOffset ToVietnamTime(DateTimeOffset utcOffset);

    // Try parse flexible client input -> DateTimeOffset (normalized to UTC)
    DateTimeOffset ParseToUtc(string input, string? clientTimeZoneId = null);

    // Format for output (client-readable ISO with +07:00)
    string FormatForClient(DateTime utc, string? tzId = "SE Asia Standard Time");
    string FormatForClient(DateTimeOffset utcOffset, string? tzId = "SE Asia Standard Time");
}
