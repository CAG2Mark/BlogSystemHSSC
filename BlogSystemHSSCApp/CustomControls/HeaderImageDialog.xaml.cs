using BlogSystemHSSC.Blog;
using Microsoft.Win32;
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

namespace BlogSystemHSSC.CustomControls
{
    /// <summary>
    /// Interaction logic for HeaderImageDialog.xaml
    /// </summary>
    public partial class HeaderImageDialog : Window
    {
        private BlogPost post { get; }

        public HeaderImageDialog(BlogPost post)
        {
            DataContext = post;
            InitializeComponent();
            this.post = post;
        }

        private void ClickClose(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ClickOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void changeImageClicked(object sender, RoutedEventArgs e)
        {
            // Initialize a new dialog
            var ofd = new OpenFileDialog();

            // Get the pictures directory
            var picturesDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            // Set the intial directory depending on whether it is valid or it is set.
            if (post.IsHeaderImageSet)
            {
                try
                {
                    ofd.InitialDirectory = System.IO.Path.GetDirectoryName(post.HeaderImageStr);
                }
                catch (Exception)
                {
                    ofd.InitialDirectory = picturesDirectory;
                }
            }
            else ofd.InitialDirectory = picturesDirectory;

            ofd.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            // Don't continue if no image was selected
            if (ofd.ShowDialog() == false) return;

            // Set the header image directory in the mode.
            post.HeaderImageStr = ofd.FileName;
        }
    }
}
