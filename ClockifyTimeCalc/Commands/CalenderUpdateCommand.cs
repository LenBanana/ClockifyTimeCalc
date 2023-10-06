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
        SettingsHandler.UpdateHolidays(holidays);
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