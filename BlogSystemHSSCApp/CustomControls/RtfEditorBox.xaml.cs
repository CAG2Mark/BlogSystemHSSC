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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlogSystemHSSC.CustomControls
{
    /// <summary>
    /// Interaction logic for RtfEditorBox.xaml
    /// </summary>
    public partial class RtfEditorBox : UserControl, INotifyPropertyChanged
    {
        public RtfEditorBox()
        {
            InitializeComponent();
        }

        private bool isBold;
        public bool IsBold
        {
            get => isBold;
            set { isBold = value; OnPropertyChanged(); }
        }


        private bool isItalic;
        public bool IsItalic
        {
            get => isItalic;
            set { isItalic = value; OnPropertyChanged(); }
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #region toggle button checked unchecked

        private void BoldChecked(object sender, RoutedEventArgs e)
        {
            EditorTextBox.Selection.ApplyPropertyValue(
                TextElement.FontWeightProperty,
                IsBold ? FontWeights.Bold : FontWeights.Normal
                );
            EditorTextBox.Focus();
        }

        private void ItalicChecked(object sender, RoutedEventArgs e)
        {
            EditorTextBox.Selection.ApplyPropertyValue(
                TextElement.FontStyleProperty, 
                IsItalic ? FontStyles.Italic : FontStyles.Normal
                );
            EditorTextBox.Focus();
        }

        #endregion

        private void checkSelection()
        {
            try
            {
                IsBold = (FontWeight)EditorTextBox.Selection.GetPropertyValue(TextElement.FontWeightProperty) == FontWeights.Bold;
            }
            catch (InvalidCastException)
            {
                IsBold = false;
            }

            try
            {
                IsItalic = (FontStyle)EditorTextBox.Selection.GetPropertyValue(TextElement.FontStyleProperty) == FontStyles.Italic;
            }
            catch (InvalidCastException)
            {
                IsItalic = false;
            }
        }

        private void EditorTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            checkSelection();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void EditorTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            checkSelection();
        }

        // TODO
        private void EditorTextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
