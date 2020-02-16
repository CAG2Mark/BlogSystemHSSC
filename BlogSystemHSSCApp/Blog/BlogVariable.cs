using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC.Blog
{

    public enum BlogVariableType
    {
        GLOBAL = 0,
        CATEGORY = 1,
        POST = 2,
        REGION = 3,
        ENDREGION = 4,
        TEMPLATE = 5,
        ENDTEMPLATE = 6,
        PAGEIND = 7,
    }

    internal class BlogVariable
    {
        public BlogVariable()
        {
        }

        public BlogVariableType Type { get; set; }
        public string VariableName { get; set; }
        public string[] Arguments { get; set; }
        public int StartPos { get; set; }
        public int EndPos { get; set; }
    }
}
