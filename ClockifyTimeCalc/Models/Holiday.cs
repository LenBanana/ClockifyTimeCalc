using ClockifyTimeCalc.Handler;

namespace ClockifyTimeCalc.Models;

/// <summary>
/// Used to store public holidays to calculate the work time correctly in the <see cref="WorkTimeHandler"/>
/// </summary>
public class Holiday
{
    public Holiday(string name, DateTime date, string location)
    {
        Name = name;
        Date = date;
        Location = location;
    }

    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string Location { get; set; }
}