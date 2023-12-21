using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseSiteForMyFriend.Core.Habra
{
    internal class HabraParser : IParser<string[]>
    {
        public string[] Parse(IHtmlDocument document)
        {
            var list = new List<string>();
            var items = document.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName == "beginners-leaft-right");
            
            foreach ( var item in items )
            {
                list.Add(item.TextContent);
            }

            return list.ToArray();
        }
    }
}
