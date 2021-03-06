﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

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
        [XmlIgnore]
        /// <summary>
        /// All the blog posts the user has saved.
        /// NOTE: This is not serialized directly. The posts will be serialized into a different folder.
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

        private int articleFontSize = 18;
        public int ArticleFontSize
        {
            get => articleFontSize;
            set => Set(ref articleFontSize, value);
        }

        private int postsPerPage = 10;
        public int PostsPerPage
        {
            get => postsPerPage;
            set => Set(ref postsPerPage, value);
        }
    }
}
