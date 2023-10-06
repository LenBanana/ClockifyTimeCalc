using ClockifyTimeCalc.Models;
using HtmlAgilityPack;

namespace ClockifyTimeCalc.Http;

public abstract class HolidayNavigation
{
    public static async Task<List<Holiday>> GetHolidays()
    {
        using var client = new HttpClient(); // Ensuring HttpClient disposal
    
        try
        {
            var response = await client.GetAsync("https://kalender-online.com/?bundesland=9&jahr=2023&feiertage=ges&feiertagsliste=true&schulferien=false&mond=0&legende=true&spalten=&reihen=#");
        
            response.EnsureSuccessStatusCode(); // Throws an exception if the HTTP response was unsuccessful

            var html = await response.Content.ReadAsStringAsync();
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var tableBody = doc.DocumentNode.SelectSingleNode("//div[@id='divlistfeiertage']/table") ?? throw new Exception("Expected HTML structure not found.");

            var rows = tableBody.SelectNodes("tr").Skip(1);
        
            return (from row in rows
                select row.SelectNodes("td")
                into cells
                where cells is { Count: >= 4 }
                let date = cells[1].InnerText
                let name = cells[2].InnerText
                let location = cells[3].InnerText
                select new Holiday(name, DateTime.Parse(date), location)).ToList();
        }
        catch (HttpRequestException e)
        {
            // Handle network-related errors here
            Console.WriteLine($"Network error: {e.Message}");
        }
        catch (FormatException e)
        {
            // Handle date parsing errors here
            Console.WriteLine($"Date parsing error: {e.Message}");
        }
        catch (Exception e)
        {
            // Handle other exceptions
            Console.WriteLine($"An error occurred: {e.Message}");
        }

        return new List<Holiday>(); // Return an empty list in case of errors
    }

}