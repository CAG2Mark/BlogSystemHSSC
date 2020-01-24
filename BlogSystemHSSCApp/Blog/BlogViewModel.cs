using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;

namespace BlogSystemHSSC.Blog
{


    /// <summary>
    /// Loads and links the blog posts with the view.
    /// </summary>
    public class BlogViewModel : BindableBase
    {

        public BlogViewModel()
        {
            if (!loadBlog()) MessageBox.Show("Could not load the blog file");
        }

        #region serialzation

        public static readonly string FilesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
            @"\Blog System Files";


        /// <summary>
        /// Loads the blog from the predefined file path and returns a boolean indicating whether the operation was successful.
        /// </summary>
        private bool loadBlog()
        {
            if (!File.Exists(FilesPath + @"\blog.xml"))
            {
                // If the blog file doesn't exist, create a new blog file. Return whether it was successful.
                return saveBlog();
            }

            try
            {
                XmlSerializer serializer =
                    new XmlSerializer(typeof(BlogModel));

                // load the file and save to the Blog property
                var fs = new FileStream(FilesPath + @"\blog.xml", FileMode.Open);
                Blog = (BlogModel)serializer.Deserialize(fs);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        /// <summary>
        /// Saves the blog to the predefined file path.
        /// </summary>
        private bool saveBlog()
        {
            try
            {
                // create the directory in case it doesn't exist
                Directory.CreateDirectory(FilesPath);

                // write the file
                var output = File.Create(FilesPath + @"\blog.xml");
                XmlSerializer formatter = new XmlSerializer(typeof(BlogModel));
                formatter.Serialize(output, Blog);

                output.Dispose();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        #endregion

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

        #region Properties

        #region UI values

        private ObservableCollection<BlogPost> openBlogPosts = new ObservableCollection<BlogPost>();
        /// <summary>
        /// All the blog posts the user currently has open in the application.
        /// </summary>
        public ObservableCollection<BlogPost> OpenBlogPosts
        {
            get => openBlogPosts;
            set => Set(ref openBlogPosts, value);
        }

        #endregion

        private BlogModel blog = new BlogModel();
        /// <summary>
        /// The blog that is currently being edited.
        /// </summary>
        public BlogModel Blog
        {
            get => blog;
            set => Set(ref blog, value);
        }

        #endregion

        #region Commands

        private bool canExecute = true;

        // Josh Smith's implementation.
        private RelayCommand createPostCommand;
        public ICommand CreatePostCommand
        {
            get
            {
                if (createPostCommand == null)
                {
                    createPostCommand = new RelayCommand(param => createPost(),
                        param => this.canExecute);
                }
                return createPostCommand;
            }
        }

        private RelayCommand blogEditedCommand;
        public ICommand BlogEditedCommand
        {
            get
            {
                if (blogEditedCommand == null)
                {
                    blogEditedCommand = new RelayCommand(param => blogEdited(),
                        param => this.canExecute);
                }
                return blogEditedCommand;
            }
        }

        private RelayCommand openBlogPostCommand;
        public ICommand OpenBlogPostCommand
        {
            get
            {
                if (openBlogPostCommand == null)
                {
                    openBlogPostCommand = new RelayCommand(param => openBlogPost(param),
                        param => this.canExecute);
                }
                return openBlogPostCommand;
            }
        }

        private RelayCommand closeBlogPostCommand;
        public ICommand CloseBlogPostCommand
        {
            get
            {
                if (closeBlogPostCommand == null)
                {
                    closeBlogPostCommand = new RelayCommand(param => closeBlogPost(param),
                        param => this.canExecute);
                }
                return closeBlogPostCommand;
            }
        }

        #endregion

        #region events

        public event EventHandler<BlogPostEventArgs> PostCreated;

        #endregion

        #region Post management
        /// <summary>
        /// Create a new post and add it to the list of posts.
        /// </summary>
        private void createPost()
        {
            // create and add the posts
            var newPost = new BlogPost();
            blog.BlogPosts.Insert(0, newPost);

            blogEdited();

            // notify the view
            PostCreated?.Invoke(this, new BlogPostEventArgs(newPost));
        }

        /// <summary>
        /// Contains the code to run when the blog has been edited.
        /// </summary>
        private void blogEdited()
        {
            // Bubble sorts all the posts once.
            BubblePostsOnce(blog.BlogPosts, blog.SortMode);
            // Saves the blog.
            saveBlog();
        }

        /// <summary>
        /// Adds a blog post editor to the TabControl in the view.
        /// </summary>
        private void openBlogPost(object param)
        {
            var post = (BlogPost)param;

            if (!openBlogPosts.Contains(post))
                openBlogPosts.Add(post);
        }

        /// <summary>
        /// Closes a blog post editor in the tabcontrol in the editor.
        /// </summary>
        private void closeBlogPost(object param)
        {
            var post = (BlogPost)param;

            if (openBlogPosts.Contains(post))
                openBlogPosts.Remove(post);
        }


        #endregion

        #region helpers

        /// <summary>
        /// Bubble sorts the blog posts once after a post has been created.
        /// </summary>
        /// <param name="collection">The collection to sort.</param>
        /// <param name="mode">The sort mode.</param>
        private void BubblePostsOnce(ObservableCollection<BlogPost> collection, PostSortMode mode)
        {
            // Note: This method is a void as ObservableCollection is a class and therefore in code is passed by reference.

            for (int i = 0; i < collection.Count - 1; i++)
            {
                // Make sure that the title is at least 1 letter long!

                // Date 1 minus as you want the greatest date to be first
                var sel1 = mode == PostSortMode.ByDate ?
                    1 - collection[i].PublishTime.Ticks : collection[i].Title.ToCharArray()[0];

                var sel2 = mode == PostSortMode.ByDate ?
                    1 - collection[i + 1].PublishTime.Ticks : collection[i + 1].Title.ToCharArray()[0];

                if (sel1 > sel2)
                {
                    var temp = collection[i];
                    collection[i] = collection[i + 1];
                    collection[i + 1] = temp;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// The EventArgs used for the PostCreated event which passes on the post that was created.
    /// </summary>
    public class BlogPostEventArgs : EventArgs
    {
        public BlogPostEventArgs(BlogPost p)
        {
            Post = p;
        }

        /// <summary>
        /// The post that was created.
        /// </summary>
        public BlogPost Post { get; set; }
    }
}
