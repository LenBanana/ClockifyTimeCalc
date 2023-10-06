namespace ClockifyTimeCalc.Interfaces;

public interface ICommand
{
    Task Execute();
    string Description { get; }
    IEnumerable<string> Identifier { get; }
}