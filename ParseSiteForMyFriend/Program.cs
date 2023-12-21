using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using ParseSiteForMyFriend.Core;
using ParseSiteForMyFriend.Core.Habra;
using System;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace ParseSiteForMyFriend
{
    internal class Program
    {
        //static List<string> listTitles = new List<string>();

        static HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            var url = "https://www.rong-chang.com/";
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return;
            }

            var source = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();

            var document = await parser.ParseDocumentAsync(source);

            var levels = await GetLevels(document);

        }

        private static async Task<Level[]> GetLevels(IHtmlDocument document)
        {
            var levels = await Task.WhenAll(document
                .GetElementsByTagName("section")
                .Where(e => e.ClassName == "beginners")
                .Where(e => e.InnerHtml.Contains("English Level"))
                .First()
                .GetElementsByTagName("ul")
                .First()
                .GetElementsByTagName("a")
                .Select(async e =>
                {
                    var ex = await GetExercises(e);
                    var level = new Level(string
                        .Join(" ", e.GetElementsByTagName("b")
                        .First().InnerHtml.Split(new[] { " ", "\t", "\n" }, StringSplitOptions.RemoveEmptyEntries)), ex);
                    return level;
                })
                .ToArray());
            
            return levels;
        }

        private static async Task<Exercise[]> GetExercises(IElement e)
        {
            var link = e.GetAttribute("href");
            if (!link.Contains("https"))
            {
                link = "https://www.rong-chang.com" + link;
            }

            var response = await client.GetAsync(link);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return null;
            }

            var source = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();

            var document = await parser.ParseDocumentAsync(source);

            List<Exercise> exercises = new();

            var exLinks = document
                .GetElementsByTagName("a")
                .ToArray();

            return null;
        }
    }
}
