using System.Text.RegularExpressions;
using ClockifyTimeCalc.Models;
using HtmlAgilityPack;
using PuppeteerSharp;

namespace ClockifyTimeCalc.Puppeteer;

public abstract class ClockifyNavigation
{
    public static async Task<IPage?> NavigateClockify()
    {
        var page = await PuppeteerBrowser.Browser.NewPageAsync();
        await page.GoToAsync("https://app.clockify.me/tracker");
        var timeout = (int)TimeSpan.FromMinutes(3).TotalMilliseconds;
        var elementHandle = await page.WaitForSelectorAsync(".cl-input-timetracker-main",
            new WaitForSelectorOptions() { Timeout = timeout });
        return elementHandle == null ? null : page;
    }

    public static async Task<List<TimeModel>> GetClockifyTimes(IPage page)
    {
        var html = await page.GetContentAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        var entries = doc.DocumentNode.SelectNodes("//entry-group/div/entry-group-header/div");
        var timeRegex = new Regex(@"(\d{2}):(\d{2}):(\d{2})");
        var times = new List<TimeModel>();
        foreach (var entry in entries)
        {
            if (entry.ChildNodes.Count <= 1) continue;
            var date = entry.ChildNodes[0].InnerText;
            var currentDate = DateTime.Now;
            switch (date.ToLower())
            {
                case "today":
                case "heute":
                    break;
                case "yesterday":
                case "gestern":
                    currentDate = currentDate.AddDays(-1);
                    break;
                default:
                    var dateRegex = new Regex(@"\w{2}\., \w{3}\. \d{1,2}");
                    var dateMatch = dateRegex.Match(date);
                    if (!dateMatch.Success) continue;
                    var dateParts = dateMatch.Value.Split(' ');
                    var month = dateParts[1].Replace(".", "");
                    var day = int.Parse(dateParts[2]);
                    var year = currentDate.Year;
                    if (currentDate.Month == 1 && month == "Dez") year--;
                    var dateString = $"{day} {month} {year}";
                    currentDate = DateTime.Parse(dateString);
                    break;
            }

            var time = entry.ChildNodes[1].InnerText;
            var match = timeRegex.Match(time);
            if (!match.Success) continue;
            var hours = int.Parse(match.Groups[1].Value);
            var minutes = int.Parse(match.Groups[2].Value);
            var seconds = int.Parse(match.Groups[3].Value);
            times.Add(new TimeModel
            {
                Date = currentDate,
                Time = new TimeSpan(hours, minutes, seconds)
            });
        }

        return times;
    }
}