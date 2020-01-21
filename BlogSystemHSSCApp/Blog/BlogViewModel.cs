using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC.Blog
{
    /// <summary>
    /// Loads and links the blog posts with the view.
    /// </summary>
    public class BlogViewModel : BindableBase
    {
        #region UI values

        #region blog editor

        private string rtfText;
        /// <summary>
        /// The RTF text to be displayed in the rich editor.
        /// </summary>
        public string RtfText
        {
            get => rtfText;
            set => Set(ref rtfText, value);
        }

        private BlogPost currentPost;
        /// <summary>
        /// The post the user is currently editing.
        /// </summary>
        public BlogPost CurrentPost
        {
            get => currentPost;
            set => Set(ref currentPost, value);
        }

        #endregion

        #endregion

        private ObservableCollection<BlogPost> blogPosts;
        /// <summary>
        /// All the blog posts the user has saved.
        /// </summary>
        public ObservableCollection<BlogPost> BlogPosts
        {
            get => blogPosts;
            set => Set(ref blogPosts, value);
        }
    }
}
