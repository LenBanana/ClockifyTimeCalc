namespace ClockifyTimeCalc.Models;

public class Settings
{
    public List<Holiday> Holidays { get; set; } = new();
    public List<TimeModel> TotalWorkedTime { get; set; } = new();
}