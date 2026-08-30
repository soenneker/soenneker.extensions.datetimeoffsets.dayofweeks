[![](https://img.shields.io/nuget/v/soenneker.extensions.datetimeoffsets.dayofweeks.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.dayofweeks/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.dayofweeks/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.dayofweeks/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.datetimeoffsets.dayofweeks.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.dayofweeks/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.dayofweeks/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.dayofweeks/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.DateTimeOffsets.DayOfWeeks

Moves a `DateTimeOffset` to the strictly previous or next occurrence of a weekday, with optional day boundaries and time-zone-aware UTC results.

## Installation

```bash
dotnet add package Soenneker.Extensions.DateTimeOffsets.DayOfWeeks
```

## Navigate using the stored offset

```csharp
using Soenneker.Extensions.DateTimeOffsets.DayOfWeeks;

DateTimeOffset monday = new(2026, 8, 31, 15, 30, 0, TimeSpan.FromHours(-4));

DateTimeOffset previousFriday = monday.ToPreviousDayOfWeek(DayOfWeek.Friday);
DateTimeOffset nextMonday = monday.ToNextDayOfWeek(DayOfWeek.Monday);
DateTimeOffset previousFridayStart = monday.ToStartOfPreviousDayOfWeek(DayOfWeek.Friday);
DateTimeOffset nextFridayEnd = monday.ToEndOfNextDayOfWeek(DayOfWeek.Friday);
```

Navigation is strict. If the input is already on the requested weekday, the result is seven days away.

`ToPreviousDayOfWeek()` and `ToNextDayOfWeek()` preserve the input time and offset. Start/end variants reset the stored clock fields to midnight or one tick before the following date while preserving that offset. These methods do not apply a named time zone's DST rules.

## Navigate in a time zone

```csharp
TimeZoneInfo eastern = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
DateTimeOffset instant = new(2026, 8, 29, 18, 0, 0, TimeSpan.Zero);

DateTimeOffset nextMondayStartUtc =
    instant.ToStartOfNextTzDayOfWeek(DayOfWeek.Monday, eastern);

DateTimeOffset previousFridayEndUtc =
    instant.ToEndOfPreviousTzDayOfWeek(DayOfWeek.Friday, eastern);
```

The time-zone variants determine the instant's local date, select the strictly previous or next matching weekday, and return the boundary with offset `+00:00`:

- `ToStartOfPreviousTzDayOfWeek()`
- `ToStartOfNextTzDayOfWeek()`
- `ToEndOfPreviousTzDayOfWeek()`
- `ToEndOfNextTzDayOfWeek()`

Starts resolve local midnight using the zone's rules. A midnight in a gap advances to the first valid local time; an ambiguous midnight selects the earlier UTC instant. Ends are one tick before the next valid local day boundary, so 23-hour and 25-hour days are handled without assuming a fixed 24-hour duration.
