using System.Reflection;
using ClockifyTimeCalc.Interfaces;

namespace ClockifyTimeCalc;

public abstract class ReflectionHandler
{
    public static Dictionary<IEnumerable<string>, ICommand> LoadCommands()
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(ICommand)) && t is { IsInterface: false, IsAbstract: false })
            .Select(Activator.CreateInstance)
            .Cast<ICommand>()
            .ToDictionary(cmd => cmd.Identifier, cmd => cmd);
    }
}