using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using ParseSiteForMyFriend.Core;
using ParseSiteForMyFriend.Core.Habra;
using System;
using System.Collections;
using System.Net;
using System.Text.RegularExpressions;

namespace ParseSiteForMyFriend
{
    public class Program
    {

        static HttpClient client = new HttpClient();

        static List<Task> downloads = new List<Task>();

        static HashSet<Guid> guids = new();

        static async Task Main(string[] args)
        {

            
            
            Task.WaitAll(downloads.ToArray());

            var result = await GetLevels();

        }

        private static Guid GetGuid()
        {
            while (true)
            {
                var id = Guid.NewGuid();
                if (guids.Contains(id))
                    continue;
                guids.Add(id);
                return id;
            }
        }

        private static string GetIndex(int index, int numCount)
        {
            return index.ToString("D" + numCount);
        }

        //private static Level[] GetLevels(IHtmlDocument document)
        //{
        //    Level[] levels = document
        //        .GetElementsByTagName("section")
        //        .Where(e => e.ClassName == "beginners")
        //        .Where(e => e.InnerHtml.Contains("English Level"))
        //        .First()
        //        .GetElementsByTagName("ul")
        //        .First()
        //        .GetElementsByTagName("a")
        //        .Select(e =>
        //        {
        //            var level = new Level(string
        //                .Join(" ", e.GetElementsByTagName("b")
        //                .First().InnerHtml.Split(new[] { " ", "\t", "\n" }, StringSplitOptions.RemoveEmptyEntries)), GetExercises(e));
        //            return level;
        //        })
        //        .ToArray();

        //    return levels;
        //}

        private static async Task<Level[]> GetLevels()
        {

            Level[] levels = new Level[8];

            //English for Children (I)
            List<Exercise> exForChildren = await GetExercises(3, "https://www.rong-chang.com/easykids/ekid/easykid{index}.htm",
                "https://www.rong-chang.com/easykids/audio/ekid{index}.mp3");
            levels[0] = new Level("English for Children (I)", exForChildren.ToArray());

            //English for Children (II)
            var exForChildren2 = await GetExercises(3, "https://www.rong-chang.com/children/kid/kid_{index}.htm",
                "https://www.rong-chang.com/children/audio/kid_{index}.mp3");
            levels[1] = new Level("English for Children (II)", exForChildren.ToArray());

            //English Level 1

            var eng1 = await GetExercises(3, "https://www.eslfast.com/begin1/b1/b1{index}.htm",
                "https://www.eslfast.com/begin1/audio/b1001c.mp3");

            //var exLvl1 = new List<Exercise>();
            //for (int i = 0; ; i++)
            //{
            //    var ex = await GetExercise(i, 3, "https://www.rong-chang.com/children/kid/kid_{index}.htm",
            //        "https://www.rong-chang.com/children/audio/kid_{index}.mp3");
            //    if (ex == null)
            //        break;
            //    exForChildren.Add(ex);
            //}
            //levels[2] = new Level("English Level 1", exForChildren.ToArray());


            return levels;
        }

        private static async Task<List<Exercise>> GetExercises(int numCount, string basePage, string baseAudio)
        {
            List<Exercise> exercises = new();
            for (int i = 1; ; i++)
            {
                var ex = await GetExercise(i, 3, basePage, baseAudio);
                if (ex == null)
                    break;
                exercises.Add(ex);
            }
            return exercises;
        }

        private static async Task<Exercise> GetExercise(int num, int numCount, string pageBaseUrl, string audioBaseUrl)
        {
            var response = await client.GetAsync(pageBaseUrl.Replace("{index}", GetIndex(num, numCount)));

            var audioUrl = audioBaseUrl.Replace("{index}", GetIndex(num, numCount));

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var guid = GetGuid();

            var audioName = guid.ToString() + ".mp3";

            var source = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();

            var document = await parser.ParseDocumentAsync(source);

            downloads.Add(DownloadAndSave(audioUrl, Directory.GetCurrentDirectory() + "\\audio", audioName));

            var name = document
                .GetElementsByClassName("main-header")
                .First()
                .GetElementsByTagName("b")
                .First()
                .InnerHtml;

            var text = string.Join(" ", document
                .GetElementsByClassName("timed all-p")
                .First()
                .InnerHtml
                .Replace("<span>", " ")
                .Replace("</span>", " ")
                .Replace("\n", " ")
                .Split(" ", StringSplitOptions.RemoveEmptyEntries));


            return new Exercise(name, guid, text);
        }

        private static async Task DownloadAndSave(string sourceFile, string destinationFolder, string destinationFileName)
        {
            Stream fileStream = await GetFileStream(sourceFile);

            if (fileStream != Stream.Null)
            {
                await SaveStream(fileStream, destinationFolder, destinationFileName);
            }
        }

        private static async Task<Stream> GetFileStream(string fileUrl)
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                Stream fileStream = await httpClient.GetStreamAsync(fileUrl);
                return fileStream;
            }
            catch (Exception ex)
            {
                return Stream.Null;
            }
        }

        private static async Task SaveStream(Stream fileStream, string destinationFolder, string destinationFileName)
        {
            if (!Directory.Exists(destinationFolder))
                Directory.CreateDirectory(destinationFolder);

            string path = Path.Combine(destinationFolder, destinationFileName);

            using (FileStream outputFileStream = new FileStream(path, FileMode.CreateNew))
            {
                await fileStream.CopyToAsync(outputFileStream);
            }
        }
    }
}
