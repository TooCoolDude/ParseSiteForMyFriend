using AngleSharp.Html.Parser;
using System.Text.Json;

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

            File.WriteAllText("serialized.json", JsonSerializer.Serialize(result));

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

        private static async Task<Level[]> GetLevels()
        {

            Level[] levels = new Level[8];

            //English for Children (I)
            List<Exercise> exForChildren = await GetExercises(3, "https://www.rong-chang.com/easykids/ekid/easykid{index}.htm");
            levels[0] = new Level("English for Children (I)", exForChildren.ToArray());

            //English for Children (II)
            var exForChildren2 = await GetExercises(3, "https://www.rong-chang.com/children/kid/kid_{index}.htm");
            levels[1] = new Level("English for Children (II)", exForChildren.ToArray());

            //English Level 1
            var exLvl1 = await GetExercises(3, "https://www.eslfast.com/begin1/b1/b1{index}.htm");
            levels[2] = new Level("English Level 1", exLvl1.ToArray());

            //English Level 2
            var exLvl2 = await GetExercises(3, "https://www.eslfast.com/begin2/b2/b2{index}.htm");
            levels[3] = new Level("English Level 2", exLvl2.ToArray());

            //English Level 3
            var exLvl3 = await GetExercises(3, "https://www.eslfast.com/begin3/b3/b3{index}.htm");
            levels[4] = new Level("English Level 3", exLvl3.ToArray());

            //English Level 4
            var exLvl4 = await GetExercises(3, "https://www.eslfast.com/begin4/b4/b4{index}.htm");
            levels[5] = new Level("English Level 4", exLvl4.ToArray());

            //English Level 5
            var exLvl5 = await GetExercises(3, "https://www.eslfast.com/begin5/b5/b5{index}.htm");
            levels[6] = new Level("English Level 5", exLvl5.ToArray());

            //English Level 6
            var exLvl6 = await GetExercises(3, "https://www.eslfast.com/begin6/b6/b6{index}.htm");
            levels[7] = new Level("English Level 6", exLvl6.ToArray());


            return levels;
        }

        private static async Task<List<Exercise>> GetExercises(int numCount, string basePage)
        {
            List<Exercise> exercises = new();
            for (int i = 1; ; i++)
            {
                var ex = await GetExercise(i, 3, basePage);
                if (ex == null)
                    break;
                exercises.Add(ex);
            }
            return exercises;
        }

        private static async Task<Exercise> GetExercise(int num, int numCount, string pageBaseUrl)
        {

            var exerciseUrl = pageBaseUrl.Replace("{index}", GetIndex(num, numCount));

            var response = await client.GetAsync(exerciseUrl);

            string audioUrl;

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var guid = GetGuid();

            var audioName = guid.ToString() + ".mp3";

            var source = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();

            var document = await parser.ParseDocumentAsync(source);

            audioUrl = document.GetElementById("audio").GetAttribute("src").Replace("..", GetAudioBaseUrl(pageBaseUrl));

            downloads.Add(DownloadAndSave(audioUrl, Directory.GetCurrentDirectory() + "\\audio", audioName));

            string name = null;

            var elementWithName = document
                .GetElementsByClassName("main-header")
                .First() ;
            if (!(elementWithName.GetElementsByTagName("b").Length == 0))
            {
                name = elementWithName.GetElementsByTagName("b").First().InnerHtml;
            }
            else
            {
                name = elementWithName.GetElementsByTagName("font").First().InnerHtml;
            }
            



            var text = string.Join(" ", document
                .GetElementsByClassName("timed all-p")
                .First()
                .InnerHtml
                .Replace("<span>", " ")
                .Replace("</span>", " ")
                .Replace("\n", " ")
                .Split(" ", StringSplitOptions.RemoveEmptyEntries));


            return new Exercise(name, guid, text, audioUrl, exerciseUrl);
        }

        private static string GetAudioBaseUrl(string pageBaseUrl)
        {
            var s = pageBaseUrl.Split('/');
            var r = string.Join('/', s.Take(s.Length - 2));
            return r;
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
            //HttpClient httpClient = new HttpClient();
            Stream fileStream = await client.GetStreamAsync(fileUrl);
            return fileStream;
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
