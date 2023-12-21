using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using ParseSiteForMyFriend.Core;
using ParseSiteForMyFriend.Core.Habra;
using System.Text.RegularExpressions;

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

            var levels = GetLevels(document);

        }

        private static Level[] GetLevels(IHtmlDocument document)
        {
            Level[] levels = document
                .GetElementsByTagName("section")
                .Where(e => e.ClassName == "beginners")
                .Where(e => e.InnerHtml.Contains("English Level"))
                .First()
                .GetElementsByTagName("ul")
                .First()
                .GetElementsByTagName("a")
                .Select(e =>
                {
                    var level = new Level(string
                        .Join(" ", e.GetElementsByTagName("b")
                        .First().InnerHtml.Split(new[] { " ", "\t", "\n" }, StringSplitOptions.RemoveEmptyEntries)), GetExercises(e));
                    return level;
                })
                .ToArray();
            
            return levels;
        }

        private static Exercise[] GetExercises(IElement e)
        {
            var link = e.GetAttribute("href");
            
            if (link.StartsWith("http"))
            {

            }
            
            else
            {

            }

            return null;
        }
    }
}
