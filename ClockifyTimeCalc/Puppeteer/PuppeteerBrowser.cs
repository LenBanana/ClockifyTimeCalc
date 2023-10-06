using PuppeteerSharp;

namespace ClockifyTimeCalc.Puppeteer;

public class PuppeteerBrowser
{
    private static PuppeteerBrowser? _instance;
    public static PuppeteerBrowser Instance => _instance ??= new PuppeteerBrowser();

    private IBrowser? Browser { get; set; }

    private PuppeteerBrowser()
    {
        InitializeAsync().Wait();
    }

    private async Task InitializeAsync()
    {
        try
        {
            var userDataDir = new DirectoryInfo("UserData");
            if (!userDataDir.Exists)
            {
                userDataDir.Create();
            }
            Browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                Args = new[] { 
                    "--no-sandbox", 
                    "--autoplay-policy=no-user-gesture-required"
                },
                IgnoredDefaultArgs = new [] { "--disable-extensions" },
                UserDataDir = userDataDir.FullName
            });
        }
        catch (Exception ex)
        {
            // Handle browser initialization errors
            Console.WriteLine($"Error initializing browser: {ex.Message}");
        }
    }

    public async Task<IPage?> GetBrowserPageAsync()
    {
        try
        {
            if (Browser != null)
            {
                var pages = await Browser.PagesAsync();
                var page = pages.FirstOrDefault();
                return page ?? await Browser.NewPageAsync();
            }
        }
        catch (Exception ex)
        {
            // Handle errors getting a page
            Console.WriteLine($"Error getting browser page: {ex.Message}");
        }
        return null;
    }

    public async Task CloseAsync()
    {
        if (Browser != null)
        {
            await Browser.CloseAsync();
            Browser = null;
        }
    }
}
