using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParseSiteForMyFriend
{
    public record Exercise(string Name, Guid Audio, string Text, string audioUrl, string exerciseUrl);
}
