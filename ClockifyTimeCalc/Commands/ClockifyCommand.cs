using ClockifyTimeCalc.Interfaces;
using ClockifyTimeCalc.Puppeteer;
using PuppeteerSharp;

namespace ClockifyTimeCalc.Commands;

public class ClockifyCommand : ICommand
{
    private static IPage? _currentPage = null;

    public async Task Execute()
    {
        _currentPage ??= await ClockifyNavigation.NavigateClockify();
        if (_currentPage == null)
        {
            Console.WriteLine("Could not navigate to Clockify");
            return;
        }

        var times = await ClockifyNavigation.GetClockifyTimes(_currentPage);
        var totalTimeSpan = new TimeSpan(times.Sum(t => t.Time.Ticks));
        var totalTime =
            $"{totalTimeSpan.Days} Tage & {totalTimeSpan.Hours}:{totalTimeSpan.Minutes}:{totalTimeSpan.Seconds}";
        Console.WriteLine($"Total time: {totalTime}");
        //These are work weeks, so add 8 hours for each day since the first date in the list then subtract the total time
        var totalWorkTime = new TimeSpan(times.First().Date.Subtract(times.Last().Date).Days * 8, 0, 0);
        //Exclude Saturday and Sunday from the total work time
        var days = times.Select(t => t.Date.DayOfWeek).Distinct().ToList();
        if (days.Contains(DayOfWeek.Saturday)) totalWorkTime = totalWorkTime.Subtract(new TimeSpan(8, 0, 0));
        if (days.Contains(DayOfWeek.Sunday)) totalWorkTime = totalWorkTime.Subtract(new TimeSpan(8, 0, 0));
        totalWorkTime = totalWorkTime.Subtract(totalTimeSpan);
        var totalWorkTimeString =
            $"{totalWorkTime.Days} Tage & {totalWorkTime.Hours}:{totalWorkTime.Minutes}:{totalWorkTime.Seconds}";
        Console.WriteLine($"Total work time: {totalWorkTimeString}");
    }

    public string Description => "Navigate to Clockify and get times";

    public IEnumerable<string> Identifier
    {
        get
        {
            yield return "browser";
            yield return "clockify";
        }
    }
}