using ClockifyTimeCalc.Handler;
using ClockifyTimeCalc.Interfaces;

namespace ClockifyTimeCalc;

internal abstract class Program
{
    static readonly Dictionary<IEnumerable<string>, ICommand> Commands = ReflectionHandler.LoadCommands();

    private static async Task Main(string[] args)
    {
        Console.WriteLine("Clockify Time Calculator");
        Console.WriteLine("Type 'help' for available commands.");
        SettingsHandler.LoadSettings();
        while (true)
        {
            Console.Write("Enter command: ");
            var commandKey = Console.ReadLine();
            if (string.IsNullOrEmpty(commandKey))
            {
                continue;
            }
            var command = Commands.FirstOrDefault(cmd => cmd.Key.Contains(commandKey)).Value;
            if (command != null)
            {
                await command.Execute();
            }
            else
            {
                Console.WriteLine("Unknown command. Type 'help' for available commands.");
            }
        }
    }
}