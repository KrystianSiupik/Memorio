using System.Globalization;
using Memorio.SharedKernel.Results;

namespace Memorio.Modules.Archive.Domain.Memories;

internal enum DatePrecision
{
    Day,
    Month,
    Year,
    Period,
}

/// <summary>When it happened, as exactly as the person remembers: the range the date stands for.</summary>
internal sealed record MemoryDate
{
    private MemoryDate(DatePrecision precision, DateOnly from, DateOnly to, TimeOnly? time)
    {
        Precision = precision;
        From = from;
        To = to;
        Time = time;
    }

    public DatePrecision Precision { get; }

    public DateOnly From { get; }

    public DateOnly To { get; }

    /// <summary>The hour, when the person knows it; only for a day. Orders memories within a day (D-303).</summary>
    public TimeOnly? Time { get; }

    /// <summary>
    /// <paramref name="from"/> and <paramref name="to"/> as yyyy-MM-dd; only a period uses both. A date is
    /// refused when it starts after <paramref name="today"/> plus a day, because the device's day can run ahead of UTC.
    /// <paramref name="time"/> as HH:mm, only with a day.
    /// </summary>
    public static Result<MemoryDate> Create(string? precision, string? from, string? to, DateOnly today, string? time = null)
    {
        if (!Enum.TryParse<DatePrecision>(precision, ignoreCase: false, out var parsed) || !Enum.IsDefined(parsed) || !TryParse(from, out var start))
        {
            return MemoryErrors.InvalidDate;
        }

        TimeOnly? hour = null;
        if (time is not null)
        {
            if (parsed != DatePrecision.Day || !TimeOnly.TryParseExact(time, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime))
            {
                return MemoryErrors.InvalidDate;
            }

            hour = parsedTime;
        }

        DateOnly first;
        DateOnly last;
        switch (parsed)
        {
            case DatePrecision.Day:
                (first, last) = (start, start);
                break;
            case DatePrecision.Month:
                first = new DateOnly(start.Year, start.Month, 1);
                last = first.AddMonths(1).AddDays(-1);
                break;
            case DatePrecision.Year:
                first = new DateOnly(start.Year, 1, 1);
                last = new DateOnly(start.Year, 12, 31);
                break;
            default:
                if (!TryParse(to, out var end) || end < start)
                {
                    return MemoryErrors.InvalidDate;
                }

                (first, last) = (start, end);
                break;
        }

        return first > today.AddDays(1) ? MemoryErrors.DateInFuture : new MemoryDate(parsed, first, last, hour);
    }

    private static bool TryParse(string? value, out DateOnly date) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
}
