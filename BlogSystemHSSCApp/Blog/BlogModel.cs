using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogSystemHSSC.Blog
{
    public enum PostSortMode
    {
        ByDate = 0,
        ByName = 1,
    }

    public class BlogModel : BindableBase
    {


        private ObservableCollection<BlogCategory> categories = new ObservableCollection<BlogCategory>();
        public ObservableCollection<BlogCategory> Categories
        {
            get => categories;
            set => Set(ref categories, value);
        }

        private ObservableCollection<BlogPost> blogPosts = new ObservableCollection<BlogPost>();
        /// <summary>
        /// All the blog posts the user has saved.
        /// </summary>
        public ObservableCollection<BlogPost> BlogPosts
        {
            get => blogPosts;
            set => Set(ref blogPosts, value);
        }


        // Redundant, no longer used
        private PostSortMode sortMode = PostSortMode.ByDate;
        public PostSortMode SortMode
        {
            get => sortMode;
            set => Set(ref sortMode, value);
        }

        private string websiteUrl = "";
        public string WebsiteUrl
        {
            get => websiteUrl;
            set => Set(ref websiteUrl, value);
        }
    }
}
