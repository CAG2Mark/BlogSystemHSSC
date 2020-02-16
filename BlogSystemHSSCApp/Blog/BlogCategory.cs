using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC.Blog
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BlogCategory : BindableBase
    {
        public BlogCategory()
        {
            // Paramaterless Constructor for Serialization
        }

        public BlogCategory(string value)
        {
            name = value;
        }

        private string name;
        [JsonProperty]
        public string Name
        {
            get => name;
            set
            {
                // Don't change the value if it equals all, archived or is empty
                Set(ref name,
                    !(value.Equals("All") || value.Equals("Archived") || string.IsNullOrWhiteSpace(value)) ? value : name);
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public event EventHandler CategoryDeleted;

        public void Delete()
        {
            CategoryDeleted?.Invoke(this, new EventArgs());
        }
    }
}
