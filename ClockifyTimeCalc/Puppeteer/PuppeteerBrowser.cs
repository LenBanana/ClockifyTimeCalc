using PuppeteerSharp;

namespace ClockifyTimeCalc.Puppeteer;

public abstract class PuppeteerBrowser
{
    public static readonly IBrowser Browser = PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
    {
        Headless = false,
        ExecutablePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe",
        Args = new[] { 
            "--no-sandbox", 
            "--autoplay-policy=no-user-gesture-required"
        },
        IgnoredDefaultArgs = new [] { "--disable-extensions" }
    }).Result;
}