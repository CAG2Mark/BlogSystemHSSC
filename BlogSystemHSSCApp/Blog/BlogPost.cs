using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;

namespace BlogSystemHSSC.Blog
{

    public class BlogPost : BindableBase
    {
        public BlogPost()
        {
            // Generate the unique 16-letter ID for Disqus. 1 in 47 octillion chance of duplicates.
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            UId = new string(Enumerable.Repeat(chars, 16)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string author = "";
        /// <summary>
        /// The author of the post.
        /// </summary>
        public string Author
        {
            get => author;
            set => Set(ref author, value);
        }

        private string title = "New Post";
        /// <summary>
        /// The title of the post.
        /// </summary>
        public string Title
        {
            get => title;
            set => Set(ref title, value);
        }

        public string GetHtmlFriendlyTitle()
        {
            var fileName = Regex.Replace(Title, "[^a-zA-Z0-9 -]", "");
            return fileName.Replace(" ", "-").ToLower();
        }

        private DateTime publishTime = DateTime.Now;
        /// <summary>
        /// The DateTime of when the post was published.
        /// </summary>
        public DateTime PublishTime
        {
            get => publishTime;
            set => Set(ref publishTime, value);
        }


        private ObservableCollection<BlogCategory> categories = new ObservableCollection<BlogCategory>();
        /// <summary>
        /// The categories of the blog post.
        /// </summary>
        public ObservableCollection<BlogCategory> Categories
        {
            get => categories;
            set => Set(ref categories, value);
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

        private bool isDraft = true;
        /// <summary>
        /// Whether or not the post is drafted.
        /// </summary>
        public bool IsDraft
        {
            get => isDraft;
            set => Set(ref isDraft, value);
        }

        /// <summary>
        /// The unique ID of this post for Disqus.
        /// </summary>
        public string UId { get; set; }

        [XmlIgnore]
        public ImageSource HeaderImageSource
        {
            get
            {
                if (!IsHeaderImageSet) return null;

                try
                {
                    return new BitmapImage(new Uri(HeaderImageStr));
                }
                catch (Exception)
                {
                    // if it is an invalid image
                    return null;
                }
            }
        }

        private string headerImageStr = "";
        public string HeaderImageStr
        {
            get => headerImageStr;
            set
            { 
                Set(ref headerImageStr, value); 
                OnPropertyChanged(nameof(HeaderImageSource));
                OnPropertyChanged(nameof(IsHeaderImageSet));
            }
        }

        public bool IsHeaderImageSet => !string.IsNullOrWhiteSpace(HeaderImageStr);

        private string headerImageCaption = "";
        public string HeaderImageCaption
        {
            get => headerImageCaption;
            set
            {
                Set(ref headerImageCaption, value);
                OnPropertyChanged(nameof(HeaderImageSource));
            }
        }


        // The document is stored during runtime as a FlowDocument/LinkedRichDocument class, but must be stored as a string
        // because FlowDocument cannot be serialized. Therefore, the property DOcument is not serialiezd and instead
        // the DocumentStr property is what's serialized. It is converted back into a FlowDocument when the application starts.
        private LinkedRichDocument document = new LinkedRichDocument(new FlowDocument());
        [XmlIgnore]
        /// <summary>
        /// The document containing all of the post data.
        /// </summary>
        public LinkedRichDocument Document
        {
            get => document;
            set => Set(ref document, value);
        }

        public string DocumentStr
        {
            get
            {
                if (Document == null) return "";
                return XamlWriter.Save(Document.AssignedDocument);
            }
       
            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Document.AssignedDocument = new FlowDocument();
                    return;
                }

                var stream = new XmlTextReader(new StringReader(value));
                Document = new LinkedRichDocument((FlowDocument)XamlReader.Load(stream));
            }
        }

        /// <summary>
        /// Generates a preview of the blog post content.
        /// </summary>
        /// <param name="maxLength">The maximum length of the preview.</param>
        /// <returns></returns>
        public string GetPreview(int maxLength)
        {
            foreach (var block in Document.AssignedDocument.Blocks)
            {
                if (block.GetType() == typeof(Paragraph))
                    foreach (var inline in ((Paragraph)block).Inlines)
                    {
                        if (inline.GetType() == typeof(Run))
                        {

                            var text = ((Run)inline).Text;
                            if (text.Length < maxLength) return text;
                            else
                            {
                                return text.Substring(0, maxLength) + "(...)";
                            }
                        }
                    }
            }

            return "";
        }
    }
}
