[![](https://img.shields.io/nuget/v/soenneker.extensions.datetimeoffsets.dayofweeks.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.dayofweeks/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.dayofweeks/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.dayofweeks/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.datetimeoffsets.dayofweeks.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.datetimeoffsets.dayofweeks/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.datetimeoffsets.dayofweeks/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.datetimeoffsets.dayofweeks/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.DateTimeOffsets.DayOfWeeks
A collection of helpful DateTimeOffset DayOfWeek extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.DateTimeOffsets.DayOfWeeks
```

## Quick start

```csharp
using Soenneker.Extensions.DateTimeOffsets.DayOfWeeks;

DateTimeOffset dateTimeOffset = DateTimeOffset.UtcNow;
var result = dateTimeOffset.ToPreviousDayOfWeek(dayOfWeek);
```

## Common operations

- `ToPreviousDayOfWeek()` - Returns the previous occurrence of `dayOfWeek` relative to `dateTimeOffset`. The result is always strictly in the past (never the same day).
- `ToNextDayOfWeek()` - Returns the next occurrence of `dayOfWeek` relative to `dateTimeOffset`. The result is always strictly in the future (never the same day).
- `ToStartOfPreviousDayOfWeek()` - Returns the start of day (00:00) for the previous occurrence of `dayOfWeek` relative to `dateTimeOffset`.
- `ToStartOfNextDayOfWeek()` - Returns the start of day (00:00) for the next occurrence of `dayOfWeek` relative to `dateTimeOffset`.
- `ToEndOfPreviousDayOfWeek()` - Returns the end of day (one tick before next day) for the previous occurrence of `dayOfWeek` relative to `dateTimeOffset`.
- `ToEndOfNextDayOfWeek()` - Returns the end of day (one tick before next day) for the next occurrence of `dayOfWeek` relative to `dateTimeOffset`.
- `ToStartOfPreviousTzDayOfWeek()` - Computes the UTC instant corresponding to the start of the previous occurrence of `dayOfWeek` in `tz`, relative to the instant `utcInstant`.
- `ToStartOfNextTzDayOfWeek()` - Computes the UTC instant corresponding to the start of the next occurrence of `dayOfWeek` in `tz`, relative to the instant `utcInstant`.
- `ToEndOfPreviousTzDayOfWeek()` - Computes the UTC instant corresponding to the end of the previous occurrence of `dayOfWeek` in `tz`, relative to the instant `utcInstant`.
- `ToEndOfNextTzDayOfWeek()` - Computes the UTC instant corresponding to the end of the next occurrence of `dayOfWeek` in `tz`, relative to the instant `utcInstant`.
