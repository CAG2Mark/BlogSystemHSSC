using BlogSystemHSSC.Blog;
using System;
using System.Collections.Generic;
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

namespace BlogSystemHSSC.CustomControls
{
    /// <summary>
    /// Interaction logic for CategoryDialog.xaml
    /// </summary>
    public partial class CategoryDialog : Window, IBindableBase
    {
        public CategoryDialog()
        {
            InitializeComponent();
        }

        private void ClickClose(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void ClickOK(object sender, RoutedEventArgs e)
        {
            if (SelectedCategory != null)
            {
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Please select a category.");
            }
        }

        private BlogCategory selectedCategory;
        public BlogCategory SelectedCategory
        {
            get => selectedCategory;
            set => Set(ref selectedCategory, value);
        }

        /// <summary>
        /// Sets the value of a property such that it can be binded to the view.
        /// </summary>
        /// <typeparam name="T">The type of the field to set.</typeparam>
        /// <param name="reference">The reference of the field to set.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="propertyName">The property name of the calling member.</param>
        public void Set<T>(ref T reference, T value, [CallerMemberName] string propertyName = null)
        {
            // set the reference value.
            reference = value;
            // call PropertyChanged on the property.
            OnPropertyChanged(propertyName);
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes PropertyChanged on a given propertyName.
        /// </summary>
        /// <param name="propertyName">The name of the property on which to call the PropertyChanged event on.</param>
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void listBoxDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCategory != null)
            {
                DialogResult = true;
            }
        }
    }
}
