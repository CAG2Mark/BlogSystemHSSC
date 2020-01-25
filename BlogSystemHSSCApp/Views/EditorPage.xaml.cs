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

        public void JumpToBlogPost(BlogPost post)
        {
            if (MasterTabControl.Items.Count > 1)
            MasterTabControl.SelectedItem = post;
        }

        #region draggable tab control

        // Credit: Adapted from 
        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/ed077477-a742-4c3d-bd4e-3efdd5dd6ba2/dragdrop-tabitem?forum=wpf

        private void TabItem_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            var tabItem = (TabItem)e.Source;

            if (tabItem == null)
                return;

            if (mouseOverCloseButton) return;

            if (Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(tabItem, tabItem, DragDropEffects.All);
            }
        }


        private void TabItem_Drop(object sender, DragEventArgs e)
        {
            var tabItemTarget = (TabItem)e.Source;
            var postTarget = (BlogPost)tabItemTarget.DataContext;

            var tabItemSource = (TabItem)e.Data.GetData(typeof(TabItem));
            var postSource = (BlogPost)tabItemSource.DataContext;

            if (!tabItemTarget.Equals(tabItemSource))
            {
                var vm = (BlogViewModel)DataContext;
                var posts = vm.OpenBlogPosts;

                int sourceIndex = posts.IndexOf(postSource);
                int targetIndex = posts.IndexOf(postTarget);

                var tmp = posts[sourceIndex];
                posts[sourceIndex] = posts[targetIndex];
                posts[targetIndex] = tmp;

                MasterTabControl.SelectedIndex = targetIndex;
            }
        }

        #endregion

        private bool mouseOverCloseButton;

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            mouseOverCloseButton = true;
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseOverCloseButton = false;
        }
    }
}
