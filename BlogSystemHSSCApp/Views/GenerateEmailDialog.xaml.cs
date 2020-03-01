using BlogSystemHSSC.Blog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
    /// Interaction logic for GenerateEmailDialog.xaml
    /// </summary>
    public partial class GenerateEmailDialog : Window, IBindableBase
    {

        private bool isSelectingMultiplePosts = false;
        public bool IsSelectingMultiplePosts
        {
            get => isSelectingMultiplePosts;
            set => Set(ref isSelectingMultiplePosts, value);
        }

        private string generatedEmail;
        public string GeneratedEmail
        {
            get => generatedEmail;
            set => Set(ref generatedEmail, value);
        }

        public GenerateEmailDialog()
        {
            InitializeComponent();
        }

        private void ClickClose(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ClickOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void nextPage(object sender, RoutedEventArgs e)
        {
            if (MasterTabControl.SelectedIndex + 1 < MasterTabControl.Items.Count)
            {
                if (MasterTabControl.SelectedIndex == 1 && SelectPostsListBox.SelectedItems.Count == 0) MessageBox.Show("Please select at least one blog post.");
                else MasterTabControl.SelectedIndex++;
            }
            if (MasterTabControl.SelectedIndex == 2) requestEmailGeneration();
        }

        private void prevPage(object sender, RoutedEventArgs e)
        {
            if (MasterTabControl.SelectedIndex > 0)
                MasterTabControl.SelectedIndex--;
        }

        private void requestEmailGeneration()
        {
            var vm = (BlogViewModel)DataContext;
            GeneratedEmail = vm.generateEmail(isSelectingMultiplePosts ?  SelectPostsListBox.SelectedItems.Cast<BlogPost>() : SelectPostsListBox.SelectedItem);
            
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

        private void copyEmailText(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GeneratedEmail);
        }
    }

    public class SelectMultipleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? SelectionMode.Multiple : SelectionMode.Single;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
