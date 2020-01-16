using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC.Blog
{

    public class BlogPost : BindableBase
    {
        private string author;
        /// <summary>
        /// The author of the post.
        /// </summary>
        public string Author
        {
            get => author;
            set => Set(ref author, value);
        }

        private DateTime publishTime;
        /// <summary>
        /// The DateTime of when the post was published.
        /// </summary>
        public DateTime PublishTime
        {
            get => publishTime;
            set => Set(ref publishTime, value);
        }

        private string headerImageName;
        /// <summary>
        /// The name (without file extension) of the header image in the folder containing all media.
        /// </summary>
        public string HeaderImageName
        {
            get => headerImageName;
            set => Set(ref headerImageName, value);
        }

        private string category;
        /// <summary>
        /// The category of the post.
        /// </summary>
        public string Category
        {
            get => category;
            set => Set(ref category, value);
        }

        private bool isArchived;
        /// <summary>
        /// Whether or not the post is archived.
        /// </summary>
        public bool IsArchived
        {
            get => isArchived;
            set => Set(ref isArchived, value);
        }

        private IEnumerable<string> rtfControls;
        /// <summary>
        /// Contains all of the RTF controls of the blog post.
        /// 
        /// Should only be iterated through.
        /// </summary>
        public IEnumerable<string> RtfControls
        {
            get => rtfControls;
            set => Set(ref rtfControls, value);
        }
    }
}
