using System;
using System.Collections.Generic;
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

        private FlowDocument document;
        [XmlIgnore]
        /// <summary>
        /// The document containing all of the post data.
        /// </summary>
        public FlowDocument Document
        {
            get => document;
            set => Set(ref document, value);
        }

        public string DocumentStr
        {
            get
            {
                if (Document == null) return "";

                Console.WriteLine(XamlWriter.Save(Document));

                return XamlWriter.Save(Document);
            }
       
            set 
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    Document = new FlowDocument();
                    return;
                }

                var stream = new XmlTextReader(new StringReader(value));
                Document = (FlowDocument)XamlReader.Load(stream);
            }
        }
    }
}
