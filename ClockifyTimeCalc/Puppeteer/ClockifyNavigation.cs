using System.Text.RegularExpressions;
using ClockifyTimeCalc.Handler;
using ClockifyTimeCalc.Models;
using HtmlAgilityPack;
using PuppeteerSharp;

namespace ClockifyTimeCalc.Puppeteer;

public abstract class ClockifyNavigation
{
    private static async Task<IPage?> GetClockifyPageAsync()
    {
        var browser = PuppeteerBrowser.Instance;
        var page = await browser.GetBrowserPageAsync();
        if (page == null) return null;

        if (page.Url.StartsWith("https://app.clockify.me/tracker")) return page;
        await page.GoToAsync("https://app.clockify.me/tracker" + SettingsHandler.GetClockifyUrlAddon());
        await page.WaitForSelectorAsync(".cl-card-header",
            new WaitForSelectorOptions() { Timeout = (int)TimeSpan.FromMinutes(3).TotalMilliseconds });

        return page;
    }

    public static async Task<bool> NavigateClockify()
    {
        var page = await GetClockifyPageAsync();
        return page != null;
    }

    public static async Task<List<TimeModel>> GetClockifyTimes()
    {
        var times = new List<TimeModel>();
        var page = await GetClockifyPageAsync();
        if (page == null) return times;

        var html = await page.GetContentAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var entries = doc.DocumentNode.SelectNodes("//entry-group/div/entry-group-header/div");

        times.AddRange(from entry in entries
            where entry.ChildNodes.Count > 1
            let date = entry.ChildNodes[0].InnerText
            let currentDate = ParseDate(date)
            let time = entry.ChildNodes[1].InnerText
            let parsedTime = ParseTime(time)
            select new TimeModel { Date = currentDate, Time = parsedTime });

        return times;
    }

    private static DateTime ParseDate(string date)
    {
        var currentDate = DateTime.Now;
        switch (date.ToLower())
        {
            case "today":
            case "heute":
                return currentDate;
            case "yesterday":
            case "gestern":
                return currentDate.AddDays(-1);
            default:
                var dateRegex = new Regex(@"\w{2}\., \w{3}\. \d{1,2}");
                var dateMatch = dateRegex.Match(date);
                if (!dateMatch.Success) return DateTime.MinValue;
                var dateParts = dateMatch.Value.Split(' ');
                var month = dateParts[1].Replace(".", "");
                var day = int.Parse(dateParts[2]);
                var year = currentDate.Year;
                if (currentDate.Month == 1 && month == "Dez") year--;
                var dateString = $"{day} {month} {year}";
                return DateTime.Parse(dateString);
        }
    }

    private static TimeSpan ParseTime(string time)
    {
        var timeRegex = new Regex(@"(\d{2}):(\d{2}):(\d{2})");
        var match = timeRegex.Match(time);
        if (!match.Success) return TimeSpan.MinValue;

        var hours = int.Parse(match.Groups[1].Value);
        var minutes = int.Parse(match.Groups[2].Value);
        var seconds = int.Parse(match.Groups[3].Value);

        return new TimeSpan(hours, minutes, seconds);
    }
}