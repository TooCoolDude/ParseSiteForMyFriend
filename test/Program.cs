using ParseSiteForMyFriend;
using System.Text.Json;

namespace test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var levels = JsonSerializer.Deserialize<Level[]>(
                File.ReadAllText("..\\..\\..\\..\\ParseSiteForMyFriend\\bin\\Debug\\net8.0\\serialized.json"));

            var audio = "..\\..\\..\\..\\ParseSiteForMyFriend\\bin\\Debug\\net8.0\\audio\\";

            foreach (var level in levels)
                foreach (var exercise in level.Exersises)
                {
                    var path = audio + exercise.Audio.ToString() + ".mp3";
                    if (!File.Exists(path))
                    {
                        var url = exercise.audioUrl;
                        Console.WriteLine(exercise.Audio.ToString());
                    }
                }
        }
    }
}
