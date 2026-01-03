using System;
using System.Diagnostics.Contracts;
using Soenneker.Extensions.DateTimeOffsets.Days;

namespace Soenneker.Extensions.DateTimeOffsets.DayOfWeeks;

/// <summary>
/// Extension methods for <see cref="DateTimeOffset"/> related to day-of-week calculations,
/// including helpers that compute day boundaries in a specified time zone while returning UTC instants.
/// </summary>
public static class DateTimeOffsetsDayOfWeeksExtension
{
    /// <summary>
    /// Returns the previous occurrence of <paramref name="dayOfWeek"/> relative to <paramref name="dateTimeOffset"/>.
    /// The result is always strictly in the past (never the same day).
    /// </summary>
    [Pure]
    public static DateTimeOffset ToPreviousDayOfWeek(this DateTimeOffset dateTimeOffset, DayOfWeek dayOfWeek)
    {
        int daysToSubtract = (dateTimeOffset.DayOfWeek - dayOfWeek + 7) % 7;
        if (daysToSubtract == 0)
            daysToSubtract = 7;
        return dateTimeOffset.AddDays(-daysToSubtract);
    }

    /// <summary>
    /// Returns the next occurrence of <paramref name="dayOfWeek"/> relative to <paramref name="dateTimeOffset"/>.
    /// The result is always strictly in the future (never the same day).
    /// </summary>
    [Pure]
    public static DateTimeOffset ToNextDayOfWeek(this DateTimeOffset dateTimeOffset, DayOfWeek dayOfWeek)
    {
        int daysToAdd = (dayOfWeek - dateTimeOffset.DayOfWeek + 7) % 7;
        if (daysToAdd == 0)
            daysToAdd = 7;
        return dateTimeOffset.AddDays(daysToAdd);
    }

    /// <summary>
    /// Returns the start of day (00:00) for the previous occurrence of <paramref name="dayOfWeek"/> relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToStartOfPreviousDayOfWeek(this DateTimeOffset dateTimeOffset, DayOfWeek dayOfWeek) =>
        dateTimeOffset.ToPreviousDayOfWeek(dayOfWeek)
                      .ToStartOfDay();

    /// <summary>
    /// Returns the start of day (00:00) for the next occurrence of <paramref name="dayOfWeek"/> relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToStartOfNextDayOfWeek(this DateTimeOffset dateTimeOffset, DayOfWeek dayOfWeek) =>
        dateTimeOffset.ToNextDayOfWeek(dayOfWeek)
                      .ToStartOfDay();

    /// <summary>
    /// Returns the end of day (one tick before next day) for the previous occurrence of <paramref name="dayOfWeek"/> relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToEndOfPreviousDayOfWeek(this DateTimeOffset dateTimeOffset, DayOfWeek dayOfWeek) =>
        dateTimeOffset.ToPreviousDayOfWeek(dayOfWeek)
                      .ToEndOfDay();

    /// <summary>
    /// Returns the end of day (one tick before next day) for the next occurrence of <paramref name="dayOfWeek"/> relative to <paramref name="dateTimeOffset"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToEndOfNextDayOfWeek(this DateTimeOffset dateTimeOffset, DayOfWeek dayOfWeek) =>
        dateTimeOffset.ToNextDayOfWeek(dayOfWeek)
                      .ToEndOfDay();

    /// <summary>
    /// Computes the UTC instant corresponding to the start of the previous occurrence of <paramref name="dayOfWeek"/>
    /// in <paramref name="tz"/>, relative to the instant <paramref name="utcInstant"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToStartOfPreviousTzDayOfWeek(this DateTimeOffset utcInstant, DayOfWeek dayOfWeek, TimeZoneInfo tz) =>
        ToStartOfTzDayOfWeekCore(utcInstant, dayOfWeek, tz, next: false);

    /// <summary>
    /// Computes the UTC instant corresponding to the start of the next occurrence of <paramref name="dayOfWeek"/>
    /// in <paramref name="tz"/>, relative to the instant <paramref name="utcInstant"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToStartOfNextTzDayOfWeek(this DateTimeOffset utcInstant, DayOfWeek dayOfWeek, TimeZoneInfo tz) =>
        ToStartOfTzDayOfWeekCore(utcInstant, dayOfWeek, tz, next: true);

    /// <summary>
    /// Computes the UTC instant corresponding to the end of the previous occurrence of <paramref name="dayOfWeek"/>
    /// in <paramref name="tz"/>, relative to the instant <paramref name="utcInstant"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToEndOfPreviousTzDayOfWeek(this DateTimeOffset utcInstant, DayOfWeek dayOfWeek, TimeZoneInfo tz) =>
        ToStartOfTzDayOfWeekCore(utcInstant, dayOfWeek, tz, next: false)
            .AddDays(1)
            .AddTicks(-1);

    /// <summary>
    /// Computes the UTC instant corresponding to the end of the next occurrence of <paramref name="dayOfWeek"/>
    /// in <paramref name="tz"/>, relative to the instant <paramref name="utcInstant"/>.
    /// </summary>
    [Pure]
    public static DateTimeOffset ToEndOfNextTzDayOfWeek(this DateTimeOffset utcInstant, DayOfWeek dayOfWeek, TimeZoneInfo tz) =>
        ToStartOfTzDayOfWeekCore(utcInstant, dayOfWeek, tz, next: true)
            .AddDays(1)
            .AddTicks(-1);

    [Pure]
    private static DateTimeOffset ToStartOfTzDayOfWeekCore(DateTimeOffset utcInstant, DayOfWeek targetDay, TimeZoneInfo tz, bool next)
    {
        if (tz is null)
            throw new ArgumentNullException(nameof(tz));

        // Normalize to a UTC instant, then convert to local to anchor on the local calendar
        DateTimeOffset utc = utcInstant.ToUniversalTime();
        DateTimeOffset local = TimeZoneInfo.ConvertTime(utc, tz);

        // Find the local date (not time) for previous/next target day-of-week
        int deltaDays = next ? (targetDay - local.DayOfWeek + 7) % 7 : (local.DayOfWeek - targetDay + 7) % 7;

        if (deltaDays == 0)
            deltaDays = 7;

        if (!next)
            deltaDays = -deltaDays;

        // Local midnight (wall-clock) for that target date
        DateTime localMidnight = new DateTime(local.Year, local.Month, local.Day, 0, 0, 0, DateTimeKind.Unspecified).AddDays(deltaDays);

        // Map wall-clock -> UTC robustly
        DateTime utcStart = ConvertLocalToUtcRobust(localMidnight, tz);
        return new DateTimeOffset(utcStart, TimeSpan.Zero);
    }

    [Pure]
    private static DateTime ConvertLocalToUtcRobust(DateTime localUnspecified, TimeZoneInfo tz)
    {
        // Spring-forward gap: advance until valid
        if (tz.IsInvalidTime(localUnspecified))
        {
            DateTime probe = localUnspecified;
            do
            {
                probe = probe.AddMinutes(1);
            }
            while (tz.IsInvalidTime(probe));

            return TimeZoneInfo.ConvertTimeToUtc(probe, tz);
        }

        // Fall-back fold: choose earlier UTC (subtract larger offset)
        if (tz.IsAmbiguousTime(localUnspecified))
        {
            TimeSpan[] offsets = tz.GetAmbiguousTimeOffsets(localUnspecified);
            TimeSpan chosen = offsets[0] >= offsets[1] ? offsets[0] : offsets[1];
            return DateTime.SpecifyKind(localUnspecified - chosen, DateTimeKind.Utc);
        }

        return TimeZoneInfo.ConvertTimeToUtc(localUnspecified, tz);
    }
}