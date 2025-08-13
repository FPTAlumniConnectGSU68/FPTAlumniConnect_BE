// TimeService.cs
using System;
using System.Globalization;

public class TimeService : ITimeService
{
    private readonly TimeZoneInfo _vietnamTz;

    public TimeService()
    {
        // Windows: "SE Asia Standard Time" ; Linux/macOS: "Asia/Ho_Chi_Minh"
        // We'll try to resolve and fallback.
        _vietnamTz = ResolveTimeZone();
    }

    private static TimeZoneInfo ResolveTimeZone()
    {
        // Try common ids for cross-platform support
        var candidates = new[] { "Asia/Ho_Chi_Minh", "SE Asia Standard Time" };
        foreach (var id in candidates)
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch { }
        }
        // Last resort: create fixed offset +7
        return TimeZoneInfo.CreateCustomTimeZone("UTC+07", TimeSpan.FromHours(7), "UTC+07", "UTC+07");
    }

    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime VietnamNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _vietnamTz);

    public DateTime ToUtc(DateTime local, string tzId = null!)
    {
        // If input DateTime.Kind == Utc, return directly
        if (local.Kind == DateTimeKind.Utc) return local;

        // assume local is in provided tz (or vietnam if null)
        var tz = tzId == null ? _vietnamTz : TimeZoneInfo.FindSystemTimeZoneById(tzId);
        var dto = new DateTimeOffset(local, tz.GetUtcOffset(local));
        return dto.UtcDateTime;
    }

    public DateTimeOffset ToUtc(DateTimeOffset local)
    {
        return local.ToUniversalTime();
    }

    public DateTime ToVietnamTime(DateTime utc)
    {
        if (utc.Kind != DateTimeKind.Utc)
            utc = DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, _vietnamTz);
    }

    public DateTimeOffset ToVietnamTime(DateTimeOffset utcOffset)
    {
        var utc = utcOffset.ToUniversalTime();
        var tzOffset = _vietnamTz.GetUtcOffset(utc.UtcDateTime);
        return new DateTimeOffset(utc.UtcDateTime + tzOffset, tzOffset);
    }

    public DateTimeOffset ParseToUtc(string input, string? clientTimeZoneId = null)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new FormatException("Empty datetime input");

        // Try parse as DateTimeOffset (accepts '2025-08-13T09:00:00+07:00' or '2025-08-13T02:00:00Z')
        if (DateTimeOffset.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dto))
        {
            return dto.ToUniversalTime();
        }

        // Try parse as DateTime (no offset) => interpret as client tz (or Vietnam default)
        if (DateTime.TryParse(input, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            // treat dt as local in client tz
            var tz = clientTimeZoneId != null ? TimeZoneInfo.FindSystemTimeZoneById(clientTimeZoneId) : _vietnamTz;
            var offset = tz.GetUtcOffset(dt);
            var dtoFromLocal = new DateTimeOffset(dt, offset);
            return dtoFromLocal.ToUniversalTime();
        }

        // try some common formats
        var formats = new[] { "yyyy-MM-dd HH:mm", "yyyy-MM-ddTHH:mm", "HH:mm", "HH:mm:ss", "yyyy-MM-ddTHH:mm:ss" };
        foreach (var f in formats)
        {
            if (DateTime.TryParseExact(input, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt2))
            {
                var tz2 = clientTimeZoneId != null ? TimeZoneInfo.FindSystemTimeZoneById(clientTimeZoneId) : _vietnamTz;
                var dtoFromLocal = new DateTimeOffset(dt2, tz2.GetUtcOffset(dt2));
                return dtoFromLocal.ToUniversalTime();
            }
        }

        throw new FormatException($"Unsupported datetime format: {input}");
    }

    public string FormatForClient(DateTime utc, string? tzId = null)
    {
        var dt = utc.Kind == DateTimeKind.Utc ? utc : DateTime.SpecifyKind(utc, DateTimeKind.Utc);
        var tz = tzId == null ? _vietnamTz : TimeZoneInfo.FindSystemTimeZoneById(tzId);
        var clientDt = TimeZoneInfo.ConvertTimeFromUtc(dt, tz);
        // return ISO 8601 with offset: e.g. "2025-08-13T16:00:00+07:00"
        var offset = tz.GetUtcOffset(dt);
        return new DateTimeOffset(clientDt, offset).ToString("yyyy-MM-ddTHH:mm:sszzz");
    }

    public string FormatForClient(DateTimeOffset utcOffset, string? tzId = null)
    {
        var utc = utcOffset.ToUniversalTime();
        return FormatForClient(utc.UtcDateTime, tzId);
    }
}
