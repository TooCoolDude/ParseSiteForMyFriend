

namespace ParseSiteForMyFriend.Core.Habra
{
    internal class HabraSettings : IParserSettings
    {
        public HabraSettings(int start, int end) 
        { 
            StartPoint = start;
            EndPoint = end;
        }
        
        public string BaseUrl { get; set; } = "https://www.rong-chang.com";
        public string Prefix { get; set; } = "page{CurrentId}";
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
    }
}
