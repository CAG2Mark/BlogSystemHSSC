using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC.Blog
{
    /// <summary>
    /// Stores all the local variables for the blog.
    /// </summary>
    public class BlogConfig : BindableBase
    {
        public BlogConfig()
        {

        }

        private string exportPath;
        public string ExportPath
        {
            get => exportPath;
            set => Set(ref exportPath, value);
        }
    }
}
