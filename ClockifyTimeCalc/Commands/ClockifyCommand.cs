using ClockifyTimeCalc.Handler;
using ClockifyTimeCalc.Interfaces;
using ClockifyTimeCalc.Puppeteer;
using PuppeteerSharp;

namespace ClockifyTimeCalc.Commands;

public class ClockifyCommand : ICommand
{
    public async Task Execute()
    {
        Console.WriteLine("Navigating to Clockify...");
        if (!await ClockifyNavigation.NavigateClockify())
        {
            Console.WriteLine("Could not navigate to Clockify");
            return;
        }

        var times = await ClockifyNavigation.GetClockifyTimes();
        SettingsHandler.UpdateTotalWorkedTime(times);
        
        var totalTimeString = WorkTimeHandler.GetTotalWorkTime(times, SettingsHandler.GetHolidays());
        Console.WriteLine(totalTimeString);
    }


    public string Description => "Navigate to Clockify and get times";

    public IEnumerable<string> Identifier
    {
        get
        {
            yield return "clockify";
            yield return "times";
            yield return "c";
        }
    }
}