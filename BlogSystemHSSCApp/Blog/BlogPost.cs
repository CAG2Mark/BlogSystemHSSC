using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
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

        private string author;
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

        private DateTime publishTime = DateTime.Now;
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

        /// <summary>
        /// The unique ID of this post for Disqus.
        /// </summary>
        public string UId { get; set; }

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
    }
}
