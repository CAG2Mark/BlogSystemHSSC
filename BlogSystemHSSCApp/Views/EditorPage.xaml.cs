using BlogSystemHSSC.Blog;
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

namespace BlogSystemHSSC.Views
{
    /// <summary>
    /// Interaction logic for EditorPage.xaml
    /// </summary>
    public partial class EditorPage : UserControl
    {
        public EditorPage()
        {
            InitializeComponent();
        }

        private void PostEditor_PostChanged(object sender, EventArgs e)
        {
            var vm = (BlogViewModel)DataContext;
            if (vm.BlogEditedCommand.CanExecute(null))
                vm.BlogEditedCommand.Execute(null);
        }

        private void CloseEditor(object sender, RoutedEventArgs e)
        {
            // get the blog post
            var obj = (Button)sender;
            var post = (BlogPost)obj.CommandParameter;

            // execute the command
            var vm = (BlogViewModel)DataContext;
            if (vm.CloseBlogPostCommand.CanExecute(post))
                vm.CloseBlogPostCommand.Execute(post);
        }
    }
}
