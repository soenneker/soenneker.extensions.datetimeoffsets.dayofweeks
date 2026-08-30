using System;
using System.Threading.Tasks;
using Soenneker.Tests.Unit;

namespace Soenneker.Extensions.DateTimeOffsets.DayOfWeeks.Tests;

public sealed class DateTimeOffsetsDayOfWeeksExtensionTests : UnitTest
{
    [Test]
    public async Task Same_weekday_moves_seven_days()
    {
        var monday = new DateTimeOffset(2026, 8, 31, 15, 0, 0, TimeSpan.Zero);

        await Assert.That(monday.ToNextDayOfWeek(DayOfWeek.Monday)).IsEqualTo(monday.AddDays(7));
        await Assert.That(monday.ToPreviousDayOfWeek(DayOfWeek.Monday)).IsEqualTo(monday.AddDays(-7));
    }

    [Test]
    public async Task Time_zone_end_uses_next_local_boundary_across_spring_forward()
    {
        TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "Eastern Standard Time" : "America/New_York");
        var saturday = new DateTimeOffset(2024, 3, 9, 12, 0, 0, TimeSpan.Zero);

        DateTimeOffset result = saturday.ToEndOfNextTzDayOfWeek(DayOfWeek.Sunday, eastern);
        var expected = new DateTimeOffset(2024, 3, 11, 4, 0, 0, TimeSpan.Zero).AddTicks(-1);

        await Assert.That(result).IsEqualTo(expected);
    }
}
