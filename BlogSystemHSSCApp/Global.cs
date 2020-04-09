using BlogSystemHSSC.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC
{
    public static class Global
    {
        public static readonly string FilesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            @"\Blog System Files";

        // temporary
        public static readonly string ExportPath = FilesPath + @"\Export\hsscstc.github.io";

        public static BlogViewModel ViewModel => (BlogViewModel)App.Current.Resources["BlogViewModel"];
    }
}
