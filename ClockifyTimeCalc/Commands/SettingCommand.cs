using ClockifyTimeCalc.Handler;
using ClockifyTimeCalc.Interfaces;

namespace ClockifyTimeCalc.Commands;

public class SettingCommand : ICommand
{
    public Task Execute()
    {
        SettingsHandler.LogSettings();
        return Task.CompletedTask;
    }

    public string Description => "Display the applied settings";
    public IEnumerable<string> Identifier
    {
        get
        {
            yield return "setting";
            yield return "s";
        }
    }
}