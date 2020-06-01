using BlogSystemHSSC.Blog;
using BlogSystemHSSC.CustomControls;
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
                (o, e) => ((PostEditor)o).blogPostChanged()
                )
            );

        /// <summary>
        /// The blog post assigned to this editor.
        /// </summary>
        public BlogPost BlogPost
        {
            get => (BlogPost)GetValue(BlogPostProperty);
            set => SetValue(BlogPostProperty, value);
        }

        private void blogPostChanged()
        {
            OnPropertyChanged(nameof(BlogPost));

            if (BlogPost != null)
                BlogPost.PropertyChanged += BlogPost_PropertyChanged;

            Editor.PostUId = BlogPost.UId;
        }

        private void BlogPost_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Brute force method to update the rich text box's document

            if (e.PropertyName.Equals(nameof(BlogPost.Title)))
            {
                Editor.setChild();
            }

            // Saves the post if the categories, author or title change.
            if (!e.PropertyName.Equals(nameof(BlogPost.Document)))
                InvokePostChange();
        }


        #endregion
        public PostEditor()
        {
            InitializeComponent();
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

        private void InvokePostChange()
        {
            PostChanged?.Invoke(this, new EventArgs());
            BlogPost.HasBeenModified = true;
        }

        private void editorLostFocus(object sender, RoutedEventArgs e)
        {
            InvokePostChange();
        }

        private void categoryRemove(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var category = (BlogCategory)button.CommandParameter;
            BlogPost.Categories.Remove(category);
            InvokePostChange();
        }

        private void categoryAdd(object sender, RoutedEventArgs e)
        {
            var d = new CategoryDialog();
            d.ShowDialog();

            if (d.DialogResult == false) return;

            // categories must be unique
            if (!BlogPost.Categories.Contains(d.SelectedCategory))
            {
                BlogPost.Categories.Add(d.SelectedCategory);
                InvokePostChange();
            }
        }

        private void setHeaderImage(object sender, RoutedEventArgs e)
        {
            var d = new HeaderImageDialog(BlogPost);
            d.ShowDialog();
        }

        private void setPublishDate(object sender, RoutedEventArgs e)
        {
            var d = new SetDateDialog(BlogPost.PublishTime);
            d.ShowDialog();

            BlogPost.PublishTime = d.NewTime;
        }
    }
}
