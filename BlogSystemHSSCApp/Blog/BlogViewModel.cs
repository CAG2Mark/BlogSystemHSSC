using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC.Blog
{
    public class BlogViewModel : BindableBase
    {
        #region UI values

        #region blog editor

        private string rtfText;
        public string RtfText
        {
            get => rtfText;
            set => Set(ref rtfText, value);
        }

        private BlogPost currentPost;
        public BlogPost CurrentPost
        {
            get => currentPost;
            set => Set(ref currentPost, value);
        }

        #endregion

        #endregion

        private ObservableCollection<BlogPost> blogPosts;
        public ObservableCollection<BlogPost> BlogPosts
        {
            get => blogPosts;
            set => Set(ref blogPosts, value);
        }

        
    }
}
