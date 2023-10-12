using System.Text;
using ClockifyTimeCalc.Models;

namespace ClockifyTimeCalc.Handler;

public abstract class WorkTimeHandler
{
    private static TimeSpan GetWorkTime(IEnumerable<TimeModel> times)
    {
        return new TimeSpan(times.Sum(t => t.Time.Ticks));
    }

    public static string GetTotalWorkTime(List<TimeModel> times, List<Holiday> holidays)
    {
        if (!times.Any()) return "No data available";

        var logBuilder = new StringBuilder();

        AppendTimeSpanInfo(times, logBuilder);

        AppendWorkdaysAndHolidaysInfo(times, holidays, out var workDays, logBuilder);

        AppendNotWorkedDaysInfo(times, holidays, workDays, logBuilder);

        AppendSummaryInfo(times, holidays, workDays, logBuilder);

        return logBuilder.ToString();
    }

    private static void AppendTimeSpanInfo(List<TimeModel> times, StringBuilder logBuilder)
    {
        logBuilder.AppendLine("----- Time Span -----");
        logBuilder.AppendLine($"From: {times.Min(t => t.Date):D}");
        logBuilder.AppendLine($"To: {times.Max(t => t.Date):D}\n");
    }

    private static void AppendWorkdaysAndHolidaysInfo(List<TimeModel> times, List<Holiday> holidays,
        out List<DateTime> workDays, StringBuilder logBuilder)
    {
        workDays = GetWorkdays(times.Min(t => t.Date), times.Max(t => t.Date)).ToList();
        var totalHolidays = GetHolidays(holidays, times.Min(t => t.Date), times.Max(t => t.Date));

        logBuilder.AppendLine("----- Workdays & Holidays -----");
        logBuilder.AppendLine($"Total Workdays: {workDays.Count}");
        logBuilder.AppendLine($"Total Holidays: {totalHolidays}\n");
    }

    private static void AppendNotWorkedDaysInfo(List<TimeModel> times, List<Holiday> holidays, List<DateTime> workDays,
        StringBuilder logBuilder)
    {
        var notWorkedDays = workDays
            .Where(d => times.All(t => t.Date.Date != d.Date) && holidays.All(h => h.Date.Date != d))
            .ToList();
        if (!notWorkedDays.Any()) return;
        logBuilder.AppendLine("----- Not Worked On -----");
        notWorkedDays.ForEach(day => logBuilder.AppendLine($"{day:D}"));
        logBuilder.AppendLine();
    }

    private static void AppendSummaryInfo(List<TimeModel> times, List<Holiday> holidays, List<DateTime> workDays,
        StringBuilder logBuilder)
    {
        var totalWorkedDays = workDays.Count - GetHolidays(holidays, times.Min(t => t.Date), times.Max(t => t.Date));
        var totalWorkTimeSpan = TimeSpan.FromHours(totalWorkedDays * SettingsHandler.GetHoursPerWorkDay());
        var totalTimeSpan = GetWorkTime(times);
        var difference = totalTimeSpan - totalWorkTimeSpan;

        logBuilder.AppendLine("----- Summary -----");
        logBuilder.AppendLine(FormatTimeSpan(totalTimeSpan, "Total work time"));
        logBuilder.AppendLine(FormatTimeSpan(totalWorkTimeSpan, "Total supposed work time"));
        logBuilder.AppendLine(FormatTimeSpan(difference, "Difference"));
    }

    private static string FormatTimeSpan(TimeSpan timeSpan, string label)
    {
        if (timeSpan == TimeSpan.Zero) return "No time tracked yet";
        var formattedTime = $@"{(timeSpan.Days > 0 ? $"{timeSpan.Days} Tage & " : "")}{timeSpan:hh\:mm\:ss}";
        return
            $"{label}:\n{formattedTime}";
    }

    private static IEnumerable<DateTime> GetWorkdays(DateTime firstDate, DateTime lastDate)
    {
        var totalDays = (lastDate - firstDate).Days + 1;
        var workdays = Enumerable.Range(0, totalDays)
            .Select(i => firstDate.AddDays(i))
            .Where(d => d.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday);
        return workdays;
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