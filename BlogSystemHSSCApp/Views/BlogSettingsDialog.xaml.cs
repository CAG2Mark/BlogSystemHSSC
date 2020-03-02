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
using System.Windows.Shapes;

namespace BlogSystemHSSC.Views
{
    /// <summary>
    /// Interaction logic for BlogSettingsDialog.xaml
    /// </summary>
    public partial class BlogSettingsDialog : Window
    {
        public BlogSettingsDialog()
        {
            InitializeComponent();
        }

        private void ClickOK(object sender, RoutedEventArgs e)
        {
            var vm = (BlogViewModel)DataContext;

            if (vm.BlogEditedCommand.CanExecute(null))
                vm.BlogEditedCommand.Execute(null);

            if (vm.SaveBlogCommand.CanExecute(null))
                vm.SaveBlogCommand.Execute(null);

            DialogResult = true;
        }
    }
}
