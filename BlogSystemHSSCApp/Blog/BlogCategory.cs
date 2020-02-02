using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC.Blog
{
    public class BlogCategory : BindableBase
    {
        public BlogCategory(string value)
        {
            this.name = value;
        }

        private string name;
        public string Name
        {
            get => name;
            set => Set(ref this.name, value);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
