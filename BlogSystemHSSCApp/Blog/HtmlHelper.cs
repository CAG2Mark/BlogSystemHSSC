using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BlogSystemHSSC.Blog
{
    public static class HtmlHelper
    {
        public static string ToUrlFileName(string text)
        {
            return Regex.Replace(text, "[^a-zA-Z0-9 -]", "").Replace(" ", "-").ToLower();
        }
    }
}
