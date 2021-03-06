﻿using BlogSystemHSSC.Blog;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = (BlogViewModel)DataContext;

            vm.PostCreated += OnPostEditRequest;
        }

        #region top bar controls

        /// <summary>
        /// Closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickClose(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// Minimizes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Toggles between the maximized and normal state of the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClickMaxMix(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        #endregion

        private void OnPostEditRequest(object sender, BlogPostEventArgs e)
        {
            var post = e.Post;
            EditorPage.JumpToBlogPost(post);
            MasterTabControl.SelectedIndex = 1;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var vm = (BlogViewModel)DataContext;
            if (vm.SaveBlogCommand.CanExecute(null))
                vm.SaveBlogCommand.Execute(null);
        }
    }
}
