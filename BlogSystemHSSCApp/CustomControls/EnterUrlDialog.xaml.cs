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
    /// Interaction logic for EnterUrlDialog.xaml
    /// </summary>
    public partial class EnterUrlDialog : Window, IBindableBase
    {
        public EnterUrlDialog()
        {
            InitializeComponent();
        }

        public EnterUrlDialog(string url)
        {
            InitializeComponent();
            Url = url;
        }

        private void ClickClose(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
        
        private void ClickOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private string url;
        public string Url
        {
            get => url;
            set => Set(ref url, value);
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

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DialogResult = true;
            }
        }
    }
}
