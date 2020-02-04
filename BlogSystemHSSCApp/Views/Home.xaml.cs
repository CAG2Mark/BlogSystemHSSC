using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BlogSystemHSSC.Blog;
using CefSharp;
using CefSharp.WinForms;

namespace BlogSystemHSSC.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        /// <summary>
        /// The browser hosted inside the window for previewing the website.
        /// </summary>
        public ChromiumWebBrowser browser;

        /// <summary>
        /// Initializer for the home page
        /// </summary>
        public Home()
        {
            CefSettings cefSettings = new CefSettings();
            cefSettings.DisableGpuAcceleration();

            Cef.Initialize(cefSettings);

            InitializeComponent();

            browser = new ChromiumWebBrowser("https://google.com");
            WfHost.Child = browser;
        }

        public event EventHandler<BlogPostEventArgs> RequestOpenPost;

        private void EditPost(object sender, RoutedEventArgs e)
        {
            // get the blog post
            var obj = (Button)sender;
            var post = (BlogPost)obj.CommandParameter;

            // execute the command
            var vm = (BlogViewModel)DataContext;
            if (vm.OpenBlogPostCommand.CanExecute(post))
                vm.OpenBlogPostCommand.Execute(post);

            RequestOpenPost?.Invoke(this, new BlogPostEventArgs(post));
        }

        #region mouseewheel speed

        // Credit: https://stackoverflow.com/questions/876994/adjust-flowdocumentreaders-scroll-increment-when-viewingmode-set-to-scroll

        private void HandleScrollSpeed(object sender, MouseWheelEventArgs e)
        {
            try
            {
                if (!(sender is DependencyObject))
                    return;

                ScrollViewer scrollViewer = (((DependencyObject)sender)).GetScrollViewer() as ScrollViewer;
                ListBox lbHost = sender as ListBox; //Or whatever your UI element is

                if (scrollViewer != null && lbHost != null)
                {
                    double scrollSpeed = 2;

                    double offset = scrollViewer.VerticalOffset - (e.Delta * scrollSpeed / 6);
                    if (offset < 0)
                        scrollViewer.ScrollToVerticalOffset(0);
                    else if (offset > scrollViewer.ExtentHeight)
                        scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
                    else
                        scrollViewer.ScrollToVerticalOffset(offset);

                    e.Handled = true;
                }
                else
                    throw new NotSupportedException("ScrollSpeed Attached Property is not attached to an element containing a ScrollViewer.");
            }
            catch (Exception ex)
            {
                //Do something...
            }
        }

        #endregion
    }

    public static class Extensions
    {
        public static DependencyObject GetScrollViewer(this DependencyObject o)
        {
            // Return the DependencyObject if it is a ScrollViewer
            if (o is ScrollViewer)
            { return o; }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);

                var result = GetScrollViewer(child);
                if (result == null)
                {
                    continue;
                }
                else
                {
                    return result;
                }
            }

            return null;
        }
    }
}
