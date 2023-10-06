using ClockifyTimeCalc.Interfaces;

namespace ClockifyTimeCalc.Commands;

public class HelpCommand : ICommand
{
    public Task Execute()
    {
        Console.WriteLine("Commands:");
        foreach (var cmd in ReflectionHandler.LoadCommands())
        {
            var identifiers = string.Join(", ", cmd.Key);
            Console.WriteLine($"{identifiers} - {cmd.Value.Description}");
        }

        return Task.CompletedTask;
    }

    public string Description => "Display this help message";
    public IEnumerable<string> Identifier
    {
        get
        {
            yield return "help";
            yield return "h";
        }
    }
}