using BlogSystemHSSC.Blog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlogSystemHSSC.Views
{
    /// <summary>
    /// Interaction logic for PostEditor.xaml
    /// </summary>
    public partial class PostEditor : UserControl, IBindableBase
    {

        #region dependency properties

        public static readonly DependencyProperty BlogPostProperty = DependencyProperty.Register(
            "BlogPost",
            typeof(BlogPost),
            typeof(PostEditor),
            new PropertyMetadata(
                new BlogPost(),
                (o, e) => ((PostEditor)o).OnPropertyChanged(nameof(BlogPost))
                )
            );

        public BlogPost BlogPost
        {
            get => (BlogPost)GetValue(BlogPostProperty);
            set => SetValue(BlogPostProperty, value);
        }


        #endregion
        public PostEditor()
        {
            InitializeComponent();

            Console.Write("New editor created");
        }

        #region IBindable members

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Set<T>(ref T reference, T value, [CallerMemberName] string propertyName = null)
        {
            // set the reference value.
            reference = value;
            // call PropertyChanged on the property.
            OnPropertyChanged(propertyName);
        }

        #endregion

        public event EventHandler PostChanged;

        private void SavePost(object sender, RoutedEventArgs e)
        {
            PostChanged?.Invoke(this, new EventArgs());
        }
    }
}
