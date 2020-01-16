using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC.Blog
{
    public class BlogConfig : BindableBase
    {
        private List<string> categories;
        public List<string> Categories
        {
            get => categories;
            set => Set(ref categories, value);
        }
    }
}
