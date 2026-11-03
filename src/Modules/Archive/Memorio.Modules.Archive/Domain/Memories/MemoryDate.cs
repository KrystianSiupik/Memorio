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
    private MemoryDate(DatePrecision precision, DateOnly from, DateOnly to)
    {
        Precision = precision;
        From = from;
        To = to;
    }

    public DatePrecision Precision { get; }

    public DateOnly From { get; }

    public DateOnly To { get; }

    /// <summary>
    /// <paramref name="from"/> and <paramref name="to"/> as yyyy-MM-dd; only a period uses both. A date is
    /// refused when it starts after <paramref name="today"/> plus a day, because the device's day can run ahead of UTC.
    /// </summary>
    public static Result<MemoryDate> Create(string? precision, string? from, string? to, DateOnly today)
    {
        if (!Enum.TryParse<DatePrecision>(precision, ignoreCase: false, out var parsed) || !Enum.IsDefined(parsed) || !TryParse(from, out var start))
        {
            return MemoryErrors.InvalidDate;
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

        return first > today.AddDays(1) ? MemoryErrors.DateInFuture : new MemoryDate(parsed, first, last);
    }

    private static bool TryParse(string? value, out DateOnly date) =>
        DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
}
