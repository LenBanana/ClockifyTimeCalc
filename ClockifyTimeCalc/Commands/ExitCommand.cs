using ClockifyTimeCalc.Interfaces;
using ClockifyTimeCalc.Puppeteer;

namespace ClockifyTimeCalc.Commands;

public class ExitCommand : ICommand
{
    public async Task Execute()
    {
        Console.WriteLine("Exiting...");
        var browser = PuppeteerBrowser.Instance;
        await browser.CloseAsync()!;
        Environment.Exit(0);
    }

    public string Description => "Exit the program";
    public IEnumerable<string> Identifier
    {
        get
        {
            yield return "exit";
            yield return "quit";
            yield return "q";
        }
    }
}