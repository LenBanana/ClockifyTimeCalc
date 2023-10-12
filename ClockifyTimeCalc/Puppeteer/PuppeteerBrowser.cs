using PuppeteerSharp;

namespace ClockifyTimeCalc.Puppeteer;

public class PuppeteerBrowser
{
    private static PuppeteerBrowser? _instance;

    public static PuppeteerBrowser Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PuppeteerBrowser();
            }

            return _instance;
        }
    }

    private IBrowser? _browser;

    private IBrowser? Browser
    {
        get
        {
            if (_browser == null)
            {
                InitializeAsync().Wait();
            }

            return _browser;
        }
        set { _browser = value; }
    }

    private PuppeteerBrowser()
    {
    } // Keep the constructor private and empty

    private async Task InitializeAsync()
    {
        try
        {
            var userDataDir = new DirectoryInfo("UserData");
            if (!userDataDir.Exists)
            {
                userDataDir.Create();
            }

            _browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                Args = new[]
                {
                    "--no-sandbox",
                    "--autoplay-policy=no-user-gesture-required"
                },
                IgnoredDefaultArgs = new[] { "--disable-extensions" },
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
                page ??= await Browser.NewPageAsync();
                await page.SetViewportAsync(new ViewPortOptions()
                {
                    Width = 1366,
                    Height = 768
                });
                return page;
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
        if (_browser != null)
        {
            await _browser.CloseAsync();
            _browser = null;
        }
    }
}