using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            browser = new ChromiumWebBrowser(Global.ViewModel.Config.ExportPath + "\\index.html");
            WfHost.Child = browser;

            Global.ViewModel.BlogExported += (o, e) =>
            {
                if (e.Success)
                {
                    Process.Start(Global.ViewModel.Config.ExportPath);
                }
                else
                {
                    MessageBox.Show("Blog could not be exported. The error is as follows: " + Environment.NewLine + Environment.NewLine +
                        e.ErrorMessage);
                }

                browser.Load(Global.ViewModel.Config.ExportPath + "\\index.html");
                browser.Refresh();
            };
        }

        public event EventHandler<BlogPostEventArgs> RequestOpenPost;

        private void editPost(object sender, RoutedEventArgs e)
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

        private void deletePost(object sender, RoutedEventArgs e)
        {
            // get the blog post
            var obj = (Button)sender;
            var post = (BlogPost)obj.CommandParameter;

            if (MessageBox.Show("Are you sure you want to delete the post \"" + post.Title + "\"?", "Delete Post", MessageBoxButton.YesNo)
            != MessageBoxResult.Yes) return;
           

            // execute the command
            var vm = (BlogViewModel)DataContext;
            if (vm.DeletePostCommand.CanExecute(post))
                vm.DeletePostCommand.Execute(post);
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
            catch (Exception)
            {
                //Do something...
            }
        }

        #endregion

        private void browserGoBack(object sender, RoutedEventArgs e)
        {
            if (browser.CanGoBack)
                browser.Back();
        }

        private void browserGoForward(object sender, RoutedEventArgs e)
        {
            if (browser.CanGoForward)
                browser.Forward();
        }

        private void browserRefresh(object sender, RoutedEventArgs e)
        {
            browser.Reload();
        }

        private void showBlogSettings(object sender, RoutedEventArgs e)
        {
            new BlogSettingsDialog().ShowDialog();
        }

        private void showGenerateEmail(object sender, RoutedEventArgs e)
        {
            new GenerateEmailDialog().ShowDialog();
        }
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
