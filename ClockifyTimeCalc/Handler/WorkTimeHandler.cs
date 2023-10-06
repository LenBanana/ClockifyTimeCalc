using ClockifyTimeCalc.Models;

namespace ClockifyTimeCalc.Handler;

public abstract class WorkTimeHandler
{
    private const int HoursPerWorkDay = 8;

    private static TimeSpan GetWorkTime(IEnumerable<TimeModel> times)
    {
        return new TimeSpan(times.Sum(t => t.Time.Ticks));
    }

    public static string GetTotalWorkTime(List<TimeModel> times, List<Holiday> holidays)
    {
        if (!times.Any()) return "No data available";
        Console.WriteLine($"Total days:\n{times.Count}");

        var firstDate = times.Min(t => t.Date);
        var lastDate = times.Max(t => t.Date);
        Console.WriteLine($"First date: \n{firstDate:D}\nLast date: \n{lastDate:D}");

        var totalDays = (lastDate - firstDate).Days + 1;
        var totalWeekends = GetWeekendDays(firstDate, lastDate);
        var totalHolidays = GetHolidays(holidays, firstDate, lastDate);

        var totalWorkDays = totalDays - totalWeekends - totalHolidays;
        var totalWorkTimeSpan = TimeSpan.FromHours(totalWorkDays * HoursPerWorkDay);

        var totalTimeSpan = GetWorkTime(times);
        var difference = totalTimeSpan - totalWorkTimeSpan;

        return $"{FormatTimeSpan(totalTimeSpan, "Total work time")}\n" +
               $"{FormatTimeSpan(totalWorkTimeSpan, "Total supposed work time")}\n" +
               $"{FormatTimeSpan(difference, "Difference")}";
    }

    private static string FormatTimeSpan(TimeSpan timeSpan, string label)
    {
        if (timeSpan == TimeSpan.Zero) return "No time tracked yet";

        var formattedTime = $@"{timeSpan.Days} Tage & {timeSpan:hh\:mm\:ss}";
        return
            $"{label}:\n{formattedTime} ({Math.Round(timeSpan.TotalHours)} hours, {Math.Round(timeSpan.TotalMinutes)} minutes)";
    }


    private static int GetWeekendDays(DateTime firstDate, DateTime lastDate)
    {
        var totalDays = (lastDate - firstDate).Days + 1;
        var totalWeekends = 0;
        for (var i = 0; i < totalDays; i++)
        {
            var date = firstDate.AddDays(i);
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                totalWeekends++;
        }

        return totalWeekends;
    }

    private static int GetHolidays(List<Holiday> holidays, DateTime firstDate, DateTime lastDate)
    {
        var totalDays = (lastDate - firstDate).Days + 1;
        var holidayDates = new HashSet<DateTime>(holidays.Select(h => h.Date.Date));
        var totalHolidays = Enumerable.Range(0, totalDays)
            .Count(i => holidayDates.Contains(firstDate.AddDays(i).Date));

        return totalHolidays;
    }
}