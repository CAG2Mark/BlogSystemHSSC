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
    }
}
