using ClockifyTimeCalc.Handler;
using ClockifyTimeCalc.Http;
using ClockifyTimeCalc.Interfaces;
using ClockifyTimeCalc.Models;

namespace ClockifyTimeCalc.Commands;

public class CalenderUpdateCommand : ICommand
{
    public async Task Execute()
    {
        Console.WriteLine("Updating calender...");
        var holidays = await HolidayNavigation.GetHolidays();
        LogHolidays(holidays);
        SettingsHandler.UpdateHolidays(holidays);
    }

    private static void LogHolidays(List<Holiday> holidays)
    {
        Console.WriteLine($"Found {holidays.Count} holidays");
    
        var workdays = 0;
        var weekends = 0;
        DateTime? nextHoliday = null;

        foreach (var holiday in holidays)
        {
            Console.WriteLine($"{holiday.Date:D} - {holiday.Name}");

            if (holiday.Date.DayOfWeek is not DayOfWeek.Saturday and not DayOfWeek.Sunday)
            {
                workdays++;
            }
            else
            {
                weekends++;
            }

            if (!nextHoliday.HasValue && holiday.Date > DateTime.Today)
            {
                nextHoliday = holiday.Date;
            }
        }

        Console.WriteLine($"{workdays} days Mo.-Fr.");
        Console.WriteLine($"{weekends} days Sa.-Su.");
        var stillToCome = holidays.Count(holiday => holiday.Date > DateTime.Today);
        Console.WriteLine($"{stillToCome} holidays still to come (next: {nextHoliday:D})");
    }


    public string Description => "Update the holiday calender";

    public IEnumerable<string> Identifier
    {
        get
        {
            yield return "update";
            yield return "u";
        }
    }
}