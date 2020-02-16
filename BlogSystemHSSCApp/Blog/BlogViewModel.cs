using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Xml.Serialization;
using Newtonsoft.Json;


namespace BlogSystemHSSC.Blog
{
    /// <summary>
    /// Loads and links the blog posts with the view and exports the blog.
    /// </summary>
    public class BlogViewModel : BindableBase
    {

        public BlogViewModel()
        {
            if (!loadBlog()) MessageBox.Show("Could not load the blog file");
            loadCategories();
        }

        #region serialzation

        public static string FilesPath => Global.FilesPath;


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

        private void loadCategories()
        {
            // Only fire OnPropertyChanged once to avoid redundant calls

            visibleCategories = new ObservableCollection<BlogCategory>();
            visibleCategories.Add(new BlogCategory("All"));
            visibleCategories.Add(new BlogCategory("Archived"));

            foreach (var s in blog.Categories)
            {
                visibleCategories.Add(s);
            }

            OnPropertyChanged(nameof(VisibleCategories));
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

                SaveStatusText = "All Changes Saved";

                return true;
            }
            catch (Exception)
            {
                SaveStatusText = "Failed to Save Blog";
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

        private ObservableCollection<BlogCategory> visibleCategories = new ObservableCollection<BlogCategory>();
        public ObservableCollection<BlogCategory> VisibleCategories
        {
            get => visibleCategories;
            set => Set(ref visibleCategories, value);
        }

        public ObservableCollection<BlogPost> VisibleBlogPosts
        {
            get
            {
                if (SelectedCategory == null) 
                    return Blog.BlogPosts;
                 
                // Special cases for all and archived since they are default and not user-added
                else if (SelectedCategory.Name.Equals("All")) 
                    return Blog.BlogPosts;

                else if (SelectedCategory.Name.Equals("Archived"))
                    return new ObservableCollection<BlogPost>(Blog.BlogPosts.Where(x => x.IsArchived));

                else
                {
                    // linq cannot be used since after serialization pass by reference is broken
                    var posts = new ObservableCollection<BlogPost>();
                    foreach (var p in Blog.BlogPosts)
                    {
                        foreach (var c in p.Categories)
                        {
                            if (c.Name.Equals(SelectedCategory.Name))
                            {
                                posts.Add(p);
                                continue;
                            }
                        }
                    }
                    return posts;
                }
            }
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

        // Josh Smith's implementation
        #region Commands

        private bool canExecute = true;

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

        private RelayCommand deletePostCommand;
        public ICommand DeletePostCommand
        {
            get
            {
                if (deletePostCommand == null)
                {
                    deletePostCommand = new RelayCommand(param => deletePost(param),
                        param => this.canExecute);
                }
                return deletePostCommand;
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

        private RelayCommand createCategoryCommand;
        public ICommand CreateCategoryCommand
        {
            get
            {
                if (createCategoryCommand == null)
                {
                    createCategoryCommand = new RelayCommand(param => createCategory(),
                        param => this.canExecute);
                }
                return createCategoryCommand;
            }
        }

        private RelayCommand deleteCategoryCommand;
        public ICommand DeleteCategoryCommand
        {
            get
            {
                if (deleteCategoryCommand == null)
                {
                    deleteCategoryCommand = new RelayCommand(param => deleteCategory(param),
                        param => this.canExecute);
                }
                return deleteCategoryCommand;
            }
        }

        private RelayCommand saveBlogCommand;
        public ICommand SaveBlogCommand
        {
            get
            {
                if (saveBlogCommand == null)
                {
                    saveBlogCommand = new RelayCommand(param => saveBlog(),
                        param => !SaveStatusText.Equals("All Changes Saved"));
                }
                return saveBlogCommand;
            }
        }

        private RelayCommand exportBlogCommand;
        public ICommand ExportBlogCommand
        {
            get
            {
                if (exportBlogCommand == null)
                {
                    exportBlogCommand = new RelayCommand(param => exportBlog(),
                        param => this.canExecute);
                }
                return exportBlogCommand;
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

            OnPropertyChanged(nameof(VisibleBlogPosts));

            blogEdited();

            // open the blog post in the view
            openBlogPost(newPost);

            // notify the view
            PostCreated?.Invoke(this, new BlogPostEventArgs(newPost));
        }

        private void deletePost(object param)
        {
            var post = (BlogPost)param;
            blog.BlogPosts.Remove(post);
            OnPropertyChanged(nameof(VisibleBlogPosts));

            OpenBlogPosts.Remove(post);
        }

        private string saveStatusText = "All Changes Saved";
        public string SaveStatusText
        {
            get => saveStatusText;
            set => Set(ref saveStatusText, value);
        }

        /// <summary>
        /// Contains the code to run when the blog has been edited.
        /// </summary>
        private void blogEdited()
        {
            // Bubble sorts all the posts once.
            BubblePostsOnce(blog.BlogPosts, blog.SortMode);
            // Saves the blog.

            OnPropertyChanged(nameof(VisibleBlogPosts));

            //saveBlog();
            SaveStatusText = "Save Changes";
        }


        /// <summary>
        /// Adds a blog post editor to the TabControl in the view.
        /// </summary>
        private void openBlogPost(object param)
        {
            var post = (BlogPost)param;
            if (!OpenBlogPosts.Contains(post)) OpenBlogPosts.Add(post);

        }



        /// <summary>
        /// Closes a blog post editor in the tabcontrol in the editor.
        /// </summary>
        private void closeBlogPost(object param)
        {
            var post = (BlogPost)param;

            if (OpenBlogPosts.Contains(post))
                OpenBlogPosts.Remove(post);
        }


        #endregion

        #region categories

        private BlogCategory selectedCategory;
        public BlogCategory SelectedCategory
        {
            get => selectedCategory;
            set
            {
                Set(ref selectedCategory, value);
                OnPropertyChanged(nameof(VisibleBlogPosts));
            }
        }

        private void createCategory()
        {
            var category = new BlogCategory("New Category");

            blog.Categories.Add(category);
            VisibleCategories.Add(category);
        }
        
        private void deleteCategory(object param)
        {
            var category = (BlogCategory)param;
            blog.Categories.Remove(category);
            visibleCategories.Remove(category);

            category.Delete();
        }

        #endregion

        #region blog export

        // temporary
        const string websiteDirectory = @"C:\Users\markn\source\repos\BlogSystemHSSC\Website";

        static int postsPerPage = 10;

        private void exportBlog()
        {

            #region pages

            // Get the blog template

            var directory = new DirectoryInfo(websiteDirectory);

            // Find the template.
            var searchPost = directory.GetFiles("blog\\blog_POST.html");
            var searchCategory = directory.GetFiles("blog\\blog_CATEGORY.html");

            if (searchPost.Length == 0)
            {
                BlogExported.Invoke(this, new BlogExportEventArgs(false, "The blog post template could not be found."));
            }
            if (searchCategory.Length == 0)
            {
                BlogExported.Invoke(this, new BlogExportEventArgs(false, "The blog category template could not be found."));
            }

            FileInfo blogTemplate = searchPost[0];
            FileInfo categoryTemplate = searchCategory[0];

            // Read the template string
            string blogTemplateStr = File.ReadAllText(blogTemplate.FullName);
            string categoryTemplateStr = File.ReadAllText(categoryTemplate.FullName);

            // Ignore drafts.
            IEnumerable<BlogPost> allPosts = Blog.BlogPosts.Where(p => !p.IsDraft);

            try
            {
                if (!Directory.Exists(exportPath)) Directory.CreateDirectory(exportPath);

                // Export every post file
                foreach (var post in allPosts)
                {
                    // Ignore drafted posts
                    if (post.IsDraft) continue;

                    // Populate the template's variables
                    string exported = replaceVariables(blogTemplateStr, post);

                    if (!string.IsNullOrWhiteSpace(post.HeaderImageStr))
                    {
                        // Export the header image
                        File.Copy(post.HeaderImageStr, $"{imagePath}\\{post.HeaderImageName}", true);
                    }

                    if (!Directory.Exists(exportPath + "\\blog")) Directory.CreateDirectory(exportPath + "\\blog");
                    // Export the page
                    File.WriteAllText(exportPath + $"\\blog\\{post.HtmlFriendlyTitle}.html", exported);
                }


                foreach (var category in VisibleCategories)
                {
                    IEnumerable<BlogPost> posts;

                    if (category.Name.Equals("All"))
                    {
                        // Ignore archived.
                        posts = allPosts.Where(p => !p.IsArchived);
                    }
                    else if (category.Name.Equals("Archived"))
                    {
                        // Only archived.
                        posts = allPosts.Where(p => p.IsArchived);
                    }
                    else
                    {
                        // Find posts where the name of the category name string is equal to the curretly exported category
                        // and ignore archived.
                        posts = allPosts.Where(
                        p => p.Categories.Where(
                            c => c.Name.Equals(category.Name))
                        .Count() > 0
                        && !p.IsArchived);
                    }

                    // export the category pages so each page has 10 posts
                    var splitPosts = SplitList(posts.ToList(), postsPerPage);

                    for (var i = 0; i < splitPosts.Length; i++)
                    {
                        var exported = replaceVariables(categoryTemplateStr, null, category, splitPosts[i], i + 1, splitPosts.Count());
                        // Special case - export all and archived posts.
                        File.WriteAllText($"{exportPath}\\blog\\{generateCategoryPageName(category, i + 1)}", exported);
                    }
                }
            }
            catch (Exception ex)
            {
                BlogExported.Invoke(this, new BlogExportEventArgs(false, ex.Message));
            }

            #endregion
            
            string json = JsonConvert.SerializeObject(allPosts.ToList());
            File.WriteAllText($"{exportPath}\\blog\\posts.json", json);


            // Copy the rest of the files over
            DirectoryCopy(websiteDirectory, exportPath, true);

            BlogExported?.Invoke(this, new BlogExportEventArgs(true));

        }

        /// <summary>
        /// Splits a into 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public List<T>[] SplitList<T>(List<T> list, int size)
        {
            if (size == 0) throw new DivideByZeroException();

            // This ensures the list size is always rounded up. 
            // Source: http://www.cs.nott.ac.uk/~psarb2/G51MPC/slides/NumberLogic.pdf
            var length = (list.Count() + size - 1) / size;

            List<T>[] listArr = new List<T>[length];

            for (int i = 0, j = 0; i < list.Count(); i += size, j++)
            {
                // Get the range between i with length "size" or the number of items remaining, whichever is smaller
                listArr[j] = list.GetRange(i, Math.Min(size, list.Count() - i));
            }
            return listArr;
        }

        private static string generateCategoryPageName(BlogCategory category, int pageNo)
        {
            return $"{HtmlHelper.ToUrlFileName($"{category.Name}-{pageNo}")}.html";
        }

        // Credit: https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-copy-directories
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private static readonly string exportPath = Global.FilesPath + "\\Export";
        private static readonly string imagePath = exportPath + "\\content\\images";

        private string replaceVariables(string text, BlogPost post = null, BlogCategory category = null, IEnumerable<BlogPost> posts = null, int currentPage = -1, int pageCount = -1)
        {
            List<BlogVariable> variables = findVariables(text, post, category);

            // This is to temporarily store all the REGION declarators in the file for matching.
            List<BlogVariable> regionDeclarators = new List<BlogVariable>();
            // This is the final list of paired regions.
            List<BlogVariable[]> pairedRegions = new List<BlogVariable[]>();

            var sb = new StringBuilder(text);


            // This is to replace all the root variables found within the text.
            for (var i = 0; i < variables.Count(); i++)
            {
                var variable = variables[i];

                string replacingText = "";
                bool replacing = true;

                switch (variable.Type)
                {
                    case BlogVariableType.GLOBAL:
                        throw new NotImplementedException();
                        break;
                    case BlogVariableType.CATEGORY:
                        {
                            if (category == null) throw new Exception("A CATEGORY variable is specified but no category is provided.");
                            replacingText = categoryVariableToText(variable, category);
                        }
                        break;
                    case BlogVariableType.POST:
                        {
                            if (post == null) throw new Exception("A POST variable is specified but no blog post is provided.");
                            replacingText = postVariableToText(variable, post);
                        }
                        break;
                    case BlogVariableType.REGION:
                    case BlogVariableType.TEMPLATE:

                        replacing = false;
                        regionDeclarators.Add(variable);

                        break;
                    case BlogVariableType.ENDREGION:
                    case BlogVariableType.ENDTEMPLATE:

                        replacing = false;

                        // Convert to array instead of collection to preserve memory
                        var search = regionDeclarators.Where(d => d.VariableName.Equals(variable.VariableName)).ToArray();
                        // This is if there was no match
                        if (search.Length < 1) throw new Exception($"The REGION declarator {variable.VariableName} could not be found.");

                        var partner = search[search.Length - 1];

                        // Pair the two declarators.
                        pairedRegions.Add(new BlogVariable[] { partner, variable });

                        // Not implemented
                        break;
                    case BlogVariableType.PAGEIND:

                        if (currentPage == -1 || category == null) throw new Exception("A PAGEIND variable is specified but the page number and category are not provided.");
                        replacingText = pageIndVariableToText(variable, category, currentPage);

                        break;
                }

                var offset = replacingText.Length - (variable.EndPos - variable.StartPos);
                // If the variable is a type that should be replaced.
                if (replacing && replacingText != null)
                {
                    sb.Remove(variable.StartPos, variable.EndPos - variable.StartPos);
                    sb.Insert(variable.StartPos, replacingText);

                    // When text is replaced, it will offset the position of the rest of the text.
                    // Offsetting helps prevent that from happening.
                    //var offset = replacingText.Length - (variable.EndPos - variable.StartPos);

                    for (var j = i + 1 ; j < variables.Count(); j++)
                    {
                        // Only change the offset of variables after the one being changed.
                        if (variables[j].StartPos > variable.EndPos)
                        {
                            variables[j].StartPos += offset;
                            variables[j].EndPos += offset;
                        }
                    }
                }
                
                /*
                var test = variables.Where(x => x.VariableName == "CATEGORY_POST_TEMPLATE").ToList();
                foreach (var item in test) 
                {
                    Console.WriteLine($"The variable {item.VariableName}, type {item.Type} has start position {item.StartPos}, {item.EndPos}. Current offset is {offset}.");
                }
                */
            }

            // The code below is used to handle all the regions in the text.

            var regionOffset = 0;

            Dictionary<string, string> TemplateDictionary = new Dictionary<string, string>();

            // Handle any specific region cases
            foreach (var region in pairedRegions)
            {

                if (region[0].Type == BlogVariableType.TEMPLATE)
                {
                    // Get the template
                    TemplateDictionary.Add(region[0].VariableName, extractRegionContent(region, sb, regionOffset));
                    // Remove the template text from the HTML as it is now useless
                    regionOffset += removeRegion(region, sb, regionOffset);
                }

                // For this case, the region should be cleared if the header image is not set.
                if (region[0].VariableName.Equals("HEADER_IMAGE"))
                {
                    // Only remove the region if a header image isn't set
                    if (post.IsHeaderImageSet) continue;

                    // Simultaneously remove text and modify the offset.
                    regionOffset += removeRegion(region, sb, regionOffset);
                }

                if (region[0].VariableName.Equals("CATEGORY_POST_AREA"))
                {
                    int tempOffset = 0;

                    if (posts != null)
                    {
                        foreach (BlogPost catPost in posts)
                        {
                            string template;
                            if (TemplateDictionary.TryGetValue(region[0].Arguments[0], out template))
                            {
                                var replacedText = replaceVariables(template, catPost);
                                // + 4 is to make sure it's placed after the comment
                                tempOffset += insertIntoRegion(region, replacedText, sb, regionOffset, 5);
                            }
                            else
                            {
                                throw new Exception($"Could not find template of name {region[0].Arguments[0]}");
                            }
                        }
                    }

                    regionOffset += tempOffset;
                }

                if (region[0].VariableName.Equals("POST_CATEGORY_TAG_AREA"))
                {
                    int tempOffset = 0;

                    if (post.Categories != null)
                    {
                        foreach (BlogCategory cat in post.Categories)
                        {
                            string template;
                            if (TemplateDictionary.TryGetValue(region[0].Arguments[0], out template))
                            {
                                var replacedText = replaceVariables(template, null, cat);
                                // + 4 is to make sure it's placed after the comment
                                tempOffset += insertIntoRegion(region, replacedText, sb, regionOffset + tempOffset, 5);
                            }
                            else
                            {
                                throw new Exception($"Could not find template of name {region[0].Arguments[0]}");
                            }
                        }
                    }

                    regionOffset += tempOffset;
                }

                if (region[0].VariableName.Equals("CATEGORY_PAGES_AREA"))
                {
                    int requiredParameters = 3;

                    if (region[0].Arguments.Length != requiredParameters)
                    {
                        throw new Exception($"Region {region[0].VariableName} requires {requiredParameters} parameters but {region[0].Arguments.Length} were provided.");
                    }

                    var indCount = Convert.ToInt32(region[0].Arguments[0]);
                    indCount = Math.Min(pageCount, indCount);

                    string template;
                    string templateSelected;

                    if (TemplateDictionary.TryGetValue(region[0].Arguments[1], out template) &&
                        TemplateDictionary.TryGetValue(region[0].Arguments[2], out templateSelected))
                    {

                        // Generate the page numbers with the current page in the middle

                        // get the midpoint, ensuring it is always rounded up
                        var midpoint = (indCount + 1) / 2;

                        var spaceToRight = indCount - midpoint;
                        var spaceToLeft = indCount - spaceToRight - 1;

                        int pageLocation = midpoint;

                        if (currentPage <= spaceToLeft)
                        {
                            pageLocation = currentPage;
                        }
                        if (pageCount - currentPage < spaceToRight)
                        {
                            pageLocation = indCount - (pageCount - currentPage);
                        }
                       

                        int?[] pages = new int?[indCount];

                        int pageLocationIndex = pageLocation - 1;

                        pages[pageLocationIndex] = currentPage;

                        // Populate to the left
                        for (int i = pageLocationIndex - 1, j = 1; 
                            i >= 0; 
                            i--, j++)
                        {
                            pages[i] = currentPage - j;
                        }

                        // Populate to the Right
                        for (int i = pageLocationIndex + 1, j = 1;
                            i < pages.Length; 
                            i++, j++)
                        {
                            pages[i] = currentPage + j;
                        }

                        foreach (var page in pages)
                        {
                            if (page == null) throw new Exception("Page was null");

                            if (page == currentPage)
                            {
                                regionOffset += insertIntoRegion(
                                    region,
                                    replaceVariables(templateSelected, null, category, null, currentPage, pageCount),
                                    sb,
                                    regionOffset,
                                    5);
                            }

                            else
                            {
                                regionOffset += insertIntoRegion(
                                    region,
                                    replaceVariables(template, null, category, null, page.GetValueOrDefault(), pageCount),
                                    sb,
                                    regionOffset,
                                    5);
                            }
                        }

                    }
                    else
                    {
                        throw new Exception("Could not find the specified indicator template.");
                    }
                }
            }

            return sb.ToString();
        }

        private int insertIntoRegion(BlogVariable[] region, string textToInsert, StringBuilder sb, int regionOffset, int commentOffset)
        {
            var pos = region[1].StartPos + regionOffset - commentOffset;

            sb.Insert(pos, textToInsert);
            return textToInsert.Length;
        }

        // Returns the offset.
        private int removeRegion(BlogVariable[] region, StringBuilder sb, int regionOffset)
        {
            // EndPos and StartPos are respectively used to remove the first -->, second <!-- 
            // and everything in between, but not uncomment anything else that is actual code
            var index1 = region[0].EndPos + regionOffset;
            var index2 = region[1].StartPos + regionOffset;

            // This is the length of text being removed.
            // Order does not matter to keep the system robust.
            // ie. ENDREGION is accidentally put before REGION
            var length = Math.Abs(index1 - index2);

            var beginIndex = Math.Min(index1, index2);

            sb.Remove(beginIndex, length);
            // Remove from the offset as we are taking away text
            return -length;
        }

        private string extractRegionContent(BlogVariable[] region, StringBuilder sb, int regionOffset)
        {
            var str = sb.ToString().Substring(region[0].EndPos + regionOffset, region[1].StartPos - region[0].EndPos).Trim();
            // remove residue comment tags
            if (str.StartsWith("-->")) str = str.Substring(3);
            if (str.EndsWith("<!--")) str = str.Substring(0, str.Length - 4);

            return str;
        }

        /// <summary>
        /// Finds the variables in the form $(TYPE VARIABLE_NAME [args]) in a string.
        /// </summary>
        /// <param name="text">The text to search.</param>
        /// <param name="post">The blog post the content comes from.</param>
        /// <param name="category">The category the content comes from.</param>
        /// <returns></returns>
        private List<BlogVariable> findVariables(string text, BlogPost post = null, BlogCategory category = null)
        {
            List<BlogVariable> variables = new List<BlogVariable>();

            var t = text.ToCharArray();
            bool checkNextForParentheses = false;

            int nestDepth = 0;

            bool inMuffledRegion = false;
            string muffledRegionName = "";

            // This entire for loop is to get all the variables.
            for (int i = 0; i < t.Length; i++)
            {
                if (t[i] == '$')
                {
                    checkNextForParentheses = true;
                    continue;
                }

                if (checkNextForParentheses)
                {
                    if (t[i] == '(')
                    {

                        if (nestDepth == 0)
                        {
                            var str = text.Substring(i, 40);
                            // The StartPos and EndPos must include the entire variable string.
                            variables.Add(new BlogVariable() { StartPos = i - 1 });
                        }

                        nestDepth++;

                    }
                    checkNextForParentheses = false;
                    continue;
                }

                // Check for end.
                // nestDepth must be greater than zero because otherwise any random closing parentheses could trigger this.
                if (nestDepth > 0)
                {
                    if (t[i] == ')')
                    {

                        nestDepth--;

                        if (nestDepth < 0)
                        {
                            throw new Exception("Nest depth went below -1.");
                        }

                        // This is when the root variable closes
                        if (nestDepth == 0)
                        {
                            // Get the variable you're editing
                            var variable = variables.Last();
                            variable.EndPos = i + 1;

                            // The variable text excluding the parentheses and dollar sign
                            string variableText = text.Substring(variable.StartPos + 2, variable.EndPos - 1 - (variable.StartPos + 2));

                            // Populate any variables inside of the variable text by recursion.
                            variableText = replaceVariables(variableText, post, category);

                            // Split the variable into its individual parts
                            string[] parts = variableText.Split(new char[] { ' ' }, 3);

                            // Detect the variable type

                            BlogVariableType type;

                            switch (parts[0].ToUpper())
                            {
                                case "GLOBAL":
                                    type = BlogVariableType.GLOBAL;
                                    break;
                                case "CATEGORY":
                                    type = BlogVariableType.CATEGORY;
                                    break;
                                case "POST":
                                    type = BlogVariableType.POST;
                                    break;
                                case "REGION":
                                    type = BlogVariableType.REGION;
                                    break;
                                case "ENDREGION":
                                    type = BlogVariableType.ENDREGION;
                                    break;
                                case "TEMPLATE":
                                    type = BlogVariableType.TEMPLATE;
                                    break;
                                case "ENDTEMPLATE":
                                    type = BlogVariableType.ENDTEMPLATE;
                                    break;
                                case "PAGEIND":
                                    type = BlogVariableType.PAGEIND;
                                    break;

                                default:
                                    throw new Exception("Could not identify variable type " + parts[0]);
                            }

                            variable.Type = type;

                            // Detect the variable name
                            variable.VariableName = parts[1];

                            #region muffled regions

                            // Muffled regions are regions where variables should not be detected

                            // The check for muffled region comes after the response to a muffled region
                            // so the muffled region declarator does not self-muffle.

                            if (inMuffledRegion)
                            {
                                if ((type != BlogVariableType.ENDREGION || !parts[1].Equals(muffledRegionName)) && type != BlogVariableType.ENDTEMPLATE)
                                {
                                    // cancel adding the last variable
                                    variables.RemoveAt(variables.Count() - 1);
                                }
                                // Exit muffled region if match.
                                else inMuffledRegion = false;
                            }

                            if ((type == BlogVariableType.REGION && isMuffledRegion(parts[1])) || type == BlogVariableType.TEMPLATE)
                            {
                                inMuffledRegion = true;
                                muffledRegionName = parts[1];
                            }

                            #endregion

                            // Detect the arguments
                            if (parts.Length > 2)
                            {
                                variable.Arguments = parts[2].Split(' ');
                            }
                            else
                            {
                                variable.Arguments = null;
                            }
                        }
                    }
                }
            }

            return variables;
        }

        private bool isMuffledRegion(string regionName)
        {
            // No longer used, residue code.
            switch(regionName)
            {
                case "CATEGORY_POST_TEMPLATE":
                    return true;
                case "CATEGORY_PAGES_TEMPLATE_SELECTED":
                    return true;
                case "CATEGORY_PAGES_TEMPLATE":
                    return true;
            }

            return false;
        }

        private string globalVariableToText(BlogVariable v)
        {
            throw new NotImplementedException();
        }

        private string categoryVariableToText(BlogVariable v, BlogCategory category)
        {
            switch (v.VariableName.ToUpper())
            {
                case "NAME":
                    if (category.Name.Equals("All")) return "All Posts";
                    else return category.Name;
                case "FILENAME":
                    return generateCategoryPageName(category,  
                        v.Arguments.Length > 0 ? Convert.ToInt32(v.Arguments[0]) : 1);
            }
            return null;
        }

        private string postVariableToText(BlogVariable v, BlogPost post)
        {
            /*
             * $(POST TITLE)
             * $(POST DATE)
             * $(POST CONTENT)
             * $(POST UID)
             * $(POST HEADER_IMAGE_NAME)
             * $(POST HEADER_IMAGE_CAPTION)
             *
             */
            switch (v.VariableName.ToUpper())
            {
                // $(POST TITLE)
                case "TITLE":
                    return post.Title;
                // $(POST UID)
                case "UID":
                    return post.UId;
                case "AUTHOR":
                    return post.Author;
                case "DATE":
                    return post.PublishTimeStr;
                case "CONTENT":
                    return flowDocumentToHtml(post.Document.AssignedDocument, post.UId);
                case "HEADER_IMAGE_NAME":
                    return post.HeaderImageName;
                case "HEADER_IMAGE_CAPTION":
                    return post.HeaderImageCaption;
                case "FILENAME":
                    return post.HtmlFriendlyTitle;
                case "CONTENT_PREVIEW":
                    var maxLength = Convert.ToInt32(v.Arguments[0]);
                    return post.GetPreview(maxLength);
                case "BOOL_HAS_HEADER_IMAGE":
                    return post.IsHeaderImageSet ? v.Arguments[0] : v.Arguments[1];


            }

            return null;
            //throw new NotImplementedException();
        }

        private string pageIndVariableToText(BlogVariable v, BlogCategory category, int page)
        {
            switch (v.VariableName.ToUpper())
            {
                case "PAGE_NO":
                    return page.ToString();
                case "PAGE_FILENAME":
                    return generateCategoryPageName(category, page);
            }
            return null;
        }

        #region document to html

        private string flowDocumentToHtml(FlowDocument document, string blogUId)
        {
            // throw new NotImplementedException();

            string html = "";

            html += blocksToHtml(document.Blocks, blogUId);
            
            return html;
        }

        private string blocksToHtml(BlockCollection blocks, string postUId)
        {
            string html = "";

            foreach (var obj in blocks)
            {
                var type = obj.GetType();
                if (type == typeof(Paragraph))
                {
                    var p = ((Paragraph)obj);

                    html += "<p>";

                    html += inlineToHtml(p.Inlines, postUId);

                    html += "</p>";
                }


                else if (obj.GetType() == typeof(List))
                {
                    var list = (List)obj;
                    var tag = list.MarkerStyle == TextMarkerStyle.Decimal ? "ol" : "ul";

                    html += $"<{tag}>";
                    foreach (ListItem li in ((List)obj).ListItems)
                    {
                        html += $"<li>{blocksToHtml(li.Blocks, postUId)}</li>";
                    }

                    html += $"</{tag}>";
                }

                // Image
                else if (obj.GetType() == typeof(BlockUIContainer))
                {
                    var container = (BlockUIContainer)obj;
                    var image = (Image)container.Child;

                    html += imageToHtml(image, postUId);
                }
            }

            return html;
        }

        private string runToHtml(Run r)
        {
            #region Tag Generator

            // Default tag.
            string tag = "span";

            // These are the only other possible tag types for RUN.
            switch (r.BaselineAlignment)
            {
                case BaselineAlignment.Subscript:
                    tag = "sub";
                    break;
                case BaselineAlignment.Superscript:
                    tag = "sup";
                    break;
            }
            #endregion

            #region Attribute Generator

            string attributes = "style=\"";

            // FONT SIZE
            // Ignore font size for subscripts and superscripts.
            // The font size is multipled by 20/18 as the default font size in the editor is 18 but on the webpage it is 20.
            if (r.BaselineAlignment == BaselineAlignment.Baseline)
                attributes += $"font-size: { Math.Round(r.FontSize * 20.0 / 18.0, 1) }px; ";
            
            // FONT STYLE
            if (r.FontStyle == FontStyles.Italic) attributes += "font-style: italic; ";

            // FONT WEIGHT
            if (r.FontWeight == FontWeights.Bold) attributes += "font-weight: bold; ";

            // TEXT DECORATIONS
            if (r.TextDecorations.Count > 0)
                if (r.TextDecorations[0].Location == TextDecorationLocation.Underline) attributes += "text-decoration: underline; ";

            attributes += "\"";

            #endregion

            string html = "";

            html += $"<{tag} {attributes}>{r.Text}</{tag}>";

            return html;
        }

        private string inlineToHtml(InlineCollection inlines, string postUId)
        {
            string html = "";

            foreach (var inline in inlines)
            {

                if (inline.GetType() == typeof(Run))
                {
                    html += runToHtml((Run)inline);
                }

                else if (inline.GetType() == typeof(Hyperlink))
                {
                    var link = (Hyperlink)inline;
                    html += $"<a target=\"_blank\" href=\"{link.NavigateUri}\">{inlineToHtml(link.Inlines, postUId)}</a>";
                }

                // Image
                else if (inline.GetType() == typeof(InlineUIContainer))
                {
                    var container = (InlineUIContainer)inline;
                    var image = (Image)container.Child;

                    html += imageToHtml(image, postUId);
                }
            }

            return html;
        }

        private string imageToHtml(Image image, string postUId)
        {
            var imageName = Path.GetFileName(image.Source.ToString());

            // Copy the image the folder.
            var fileName = $"{postUId}_{imageName}";
            if (!Directory.Exists(imagePath)) Directory.CreateDirectory(imagePath);
            File.Copy(
                image.Source.ToString().Replace("file:///", ""), $"{imagePath}\\{fileName}", true);

            return  $"<img src=\"content/images/{fileName}\" />";
        }

        #endregion

        public event EventHandler<BlogExportEventArgs> BlogExported;

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
        public BlogPost Post { get; }
    }

    /// <summary>
    /// The EventArgs used for blog export events.
    /// </summary>
    public class BlogExportEventArgs : EventArgs
    {

        public BlogExportEventArgs(bool success)
        {
            Success = success;
        }

        public BlogExportEventArgs(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Whether the export was successful.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// The error message associated with the failure of an export.
        /// </summary>
        public string ErrorMessage { get; } = "";
    }
}
