using BlogSystemHSSC.Blog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BlogSystemHSSC.Views
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window, IBindableBase
    {
        public SplashScreen()
        {
            InitializeComponent();

            BlogList = BlogViewModel.GetBlogList();
        }

        private void ClickClose(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private ObservableCollection<string> blogList;
        public ObservableCollection<string> BlogList
        {
            get => blogList;
            set => Set(ref blogList, value);
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

        private void BlogSelected(object sender, RoutedEventArgs e)
        {
            var folderName = (string)((Button)sender).CommandParameter;
            Global.ViewModel.Initialize(folderName);

            Close();

            var mw = new MainWindow();
            mw.Show();
        }
    }
}
