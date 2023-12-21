using AngleSharp.Html.Dom;

namespace ParseSiteForMyFriend.Core
{
    public interface IParser<T> where T : class
    {
        T Parse(IHtmlDocument document);
    }
}
