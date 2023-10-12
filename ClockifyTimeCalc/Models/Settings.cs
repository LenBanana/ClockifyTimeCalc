namespace ClockifyTimeCalc.Models;

public class Settings
{
    public string ClockifyUrlAddon { get; set; } = "?page=1&limit=200";
    public string HolidayUrl { get; set; } = "https://kalender-online.com/?bundesland=9&jahr=2023&feiertage=ges&feiertagsliste=true&schulferien=false&mond=0&legende=true&spalten=&reihen=#";
    public int HoursPerWorkDay { get; set; } = 8;
    public List<Holiday> Holidays { get; set; } = new();
    public List<TimeModel> TotalWorkedTime { get; set; } = new();
}