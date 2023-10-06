using ClockifyTimeCalc.Models;
using Newtonsoft.Json;

namespace ClockifyTimeCalc.Handler;

public abstract class SettingsHandler
{
    private static Settings Settings { get; set; } = new();

    private static void SaveSettings()
    {
        var json = JsonConvert.SerializeObject(Settings);
        File.WriteAllText("settings.json", json);
    }

    public static List<Holiday> GetHolidays()
    {
        return Settings.Holidays;
    }

    public static void LoadSettings()
    {
        if (!File.Exists("settings.json"))
        {
            SaveSettings();
            return;
        }

        var json = File.ReadAllText("settings.json");
        var settings = JsonConvert.DeserializeObject<Settings>(json);
        if (settings == null) return;
        Settings = settings;
        if (Settings.TotalWorkedTime?.Count == 0 || Settings.TotalWorkedTime == null) return;
        var timeString = WorkTimeHandler.GetTotalWorkTime(Settings.TotalWorkedTime, Settings.Holidays);
        Console.WriteLine(timeString);
    }

    public static void UpdateHolidays(List<Holiday> holidays)
    {
        Settings.Holidays = holidays;
        SaveSettings();
    }

    public static void UpdateTotalWorkedTime(List<TimeModel> times)
    {
        Settings.TotalWorkedTime = times;
        SaveSettings();
    }
}