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

                // Don't use this code. It replaces items which is not supported by TabControlEx.

                // var tmp = posts[sourceIndex];
                // posts[sourceIndex] = posts[targetIndex];
                // posts[targetIndex] = tmp;

                posts.RemoveAt(sourceIndex);
                posts.Insert(sourceIndex, postTarget);

                posts.RemoveAt(targetIndex);
                if (targetIndex > posts.Count - 1)
                {
                    posts.Add(postSource);
                }
                else
                {
                    posts.Insert(targetIndex, postSource);
                }

                MasterTabControl.SelectedIndex = targetIndex;
            }
        }

        #endregion

        #region tab control functionality

        private bool mouseOverCloseButton;

        private void CloseButton_MouseEnter(object sender, MouseEventArgs e)
        {
            mouseOverCloseButton = true;
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            mouseOverCloseButton = false;
        }

        #endregion

        private void EditPost(object sender, RoutedEventArgs e)
        {
            // get the blog post
            var obj = (Button)sender;
            var post = (BlogPost)obj.CommandParameter;

            // execute the command
            var vm = (BlogViewModel)DataContext;
            if (vm.OpenBlogPostCommand.CanExecute(post))
                vm.OpenBlogPostCommand.Execute(post);

            JumpToBlogPost(post);
        }

        #region categories view

        private async void CategoryClick(object sender, MouseButtonEventArgs e)
        {
            // detect double click, should only fire on double click
            if (e.ClickCount != 2) return;

            var grid = (Grid)sender;

            // These are categories that should not be edited
            var categoryName = ((BlogCategory)grid.DataContext).Name;
            if (categoryName.Equals("All") || categoryName.Equals("Archived")) return;

            var editGrid = (Grid)grid.Children[0];

            editGrid.Visibility = Visibility.Visible;

            // give the UI time to respond before selecting all text in textbox

            await Task.Delay(1);

            // select all text
            var textBox = (TextBox)editGrid.Children[0];
            textBox.Focus();
            textBox.SelectAll();
        }

        private void CategoryEditDone(object sender, RoutedEventArgs e)
        {
            var editGrid = (Grid)((Button)sender).CommandParameter;
            editGrid.Visibility = Visibility.Collapsed;
        }

        private bool hasPendingCategoryCreate;

        private void CategoryCreated(object sender, RoutedEventArgs e)
        {
            hasPendingCategoryCreate = true;
        }

        private async void CategoryLoaded(object sender, RoutedEventArgs e)
        {
            if (!hasPendingCategoryCreate) return;

            var grid = (Grid)sender;

            var editGrid = (Grid)grid.Children[0];

            editGrid.Visibility = Visibility.Visible;

            // give the UI time to respond before selecting all text in textbox

            await Task.Delay(1);

            // select all text
            var textBox = (TextBox)editGrid.Children[0];
            textBox.Focus();
            textBox.SelectAll();

            hasPendingCategoryCreate = false;
        }


        private void CategoryTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape || e.Key == Key.Enter)
            {
                var textBox = (TextBox)sender;
                var editGrid = (Grid)textBox.Parent;
                editGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void CategoryTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (isMouseOverDeleteButton) return;

            var textBox = (TextBox)sender;
            var editGrid = (Grid)textBox.Parent;
            editGrid.Visibility = Visibility.Collapsed;
        }

        private void DeleteCategoryClick(object sender, RoutedEventArgs e)
        {
            var category = (BlogCategory)((Button)sender).CommandParameter;

            if (MessageBox.Show("Are you sure you want to delete the category \"" + category.ToString() + "\"?", "Delete Category", MessageBoxButton.YesNo)
                == MessageBoxResult.Yes)
            {
                var vm = (BlogViewModel)DataContext;
                if (vm.DeleteCategoryCommand.CanExecute(category))
                    vm.DeleteCategoryCommand.Execute(category);
            }
        }

        bool isMouseOverDeleteButton;

        private void DeleteButtonMouseEnter(object sender, MouseEventArgs e)
        {
            isMouseOverDeleteButton = true;
        }

        private void DeleteButtonMouseLeave(object sender, MouseEventArgs e)
        {
            isMouseOverDeleteButton = false;
        }

        #endregion


    }
}
