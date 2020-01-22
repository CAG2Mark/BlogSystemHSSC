using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace BlogSystemHSSC.CustomControls
{
    /// <summary>
    /// Interaction logic for RtfEditorBox.xaml
    /// </summary>
    public partial class RichTextEditor : UserControl, IBindableBase
    {

        #region dependency properties

        public static readonly DependencyProperty RichDocumentProperty =
            DependencyProperty.Register(
                "RichDocument", 
                typeof(FlowDocument), 
                typeof(RichTextEditor),
                new PropertyMetadata(
                    null, 
                    (o, e) => ((RichTextEditor)o).richDocumentChanged()
                    )
                );

        public FlowDocument RichDocument
        {
            get => (FlowDocument)GetValue(RichDocumentProperty);
            set => SetValue(RichDocumentProperty, value);
        }

        bool wasChangedInternally = false;
        private void richDocumentChanged()
        {
            OnPropertyChanged(nameof(RichDocument));

            // prevent infinite loop of setting then callback
            if (!wasChangedInternally)
            {
                EditorTextBox.Document = RichDocument;
            }
            else wasChangedInternally = false;
        }

        #endregion

        /// <summary>
        /// Provides an interface for the user to easily edit rich text.
        /// </summary>
        public RichTextEditor()
        {
            InitializeComponent();
            EditorTextBox.AutoWordSelection = false;
        }

        private double selFontSize = 18;
        public double SelFontSize
        {
            get => selFontSize;
            set => Set(ref selFontSize, value);
        }

        private bool isBold;
        public bool IsBold
        {
            get => isBold;
            set => Set(ref isBold, value);
        }

        private bool isItalic;
        public bool IsItalic
        {
            get => isItalic;
            set => Set(ref isItalic, value);
        }

        private bool isUnderline;
        public bool IsUnderline
        {
            get => isUnderline;
            set => Set(ref isUnderline, value);
        }

        private bool isSubscript;
        public bool IsSubscript
        {
            get => isSubscript;
            set => Set(ref isSubscript, value);
        }

        private bool isSuperscript;
        public bool IsSuperscript
        {
            get => isSuperscript;
            set => Set(ref isSuperscript, value);
        }

        private void scvMouseWheeled(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void fontSizeChanged(object sender, EventArgs e)
        {
            EditorTextBox.Selection.ApplyPropertyValue(
                TextElement.FontSizeProperty,
                SelFontSize
            );
            EditorTextBox.Focus();
        }

        #region toggle button clicked

        private void boldClicked(object sender, RoutedEventArgs e)
        {
            EditorTextBox.Selection.ApplyPropertyValue(
                TextElement.FontWeightProperty,
                IsBold ? FontWeights.Bold : FontWeights.Normal
                );
            EditorTextBox.Focus();
        }

        private void italicClicked(object sender, RoutedEventArgs e)
        {
            EditorTextBox.Selection.ApplyPropertyValue(
                TextElement.FontStyleProperty, 
                IsItalic ? FontStyles.Italic : FontStyles.Normal
                );
            EditorTextBox.Focus();
        }

        private void underlineClicked(object sender, RoutedEventArgs e)
        {
            EditorTextBox.Selection.ApplyPropertyValue(
                Inline.TextDecorationsProperty,
                IsUnderline ? TextDecorations.Underline : null
                );
            EditorTextBox.Focus();
        }

        private void alignClicked(object sender, RoutedEventArgs e)
        {
            // update toggled states
            selectAlignButton(sender);

            // re-focus
            EditorTextBox.Focus();
        }

        private void subscriptClicked(object sender, RoutedEventArgs e)
        {
            EditorTextBox.Selection.ApplyPropertyValue(
                Inline.BaselineAlignmentProperty,
                IsSubscript ? BaselineAlignment.Subscript : BaselineAlignment.Baseline
                );
            EditorTextBox.Focus();

            checkSelection();
        }


        private void superscriptClicked(object sender, RoutedEventArgs e)
        {
            EditorTextBox.Selection.ApplyPropertyValue(
                Inline.BaselineAlignmentProperty,
                IsSuperscript ? BaselineAlignment.Superscript : BaselineAlignment.Baseline
                );
            EditorTextBox.Focus();

            checkSelection();
        }

        #endregion

        #region buttonsClicked

        private void selectAlignButton(object button)
        {
            ToggleAlignCenterButton.IsChecked = false;
            ToggleAlignJustifyButton.IsChecked = false;
            ToggleAlignLeftButton.IsChecked = false;
            ToggleAlignRightButton.IsChecked = false;

            if (button == null) return;

            ((ToggleButton)button).IsChecked = true;
        }

        private void addListButtonClicked(object sender, RoutedEventArgs e)
        {
            EditorTextBox.Focus();
        }

        private void addImageButtonClicked(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

            if (dialog.ShowDialog() == false) return;

            // Credit: https://social.msdn.microsoft.com/Forums/vstudio/en-US/e4b322c2-2553-4802-bd0d-fe7ef7e90b37/inserting-image-into-richtextbox-control?forum=wpf

            Paragraph p = new Paragraph();

            try
            {
                var bitmap = new BitmapImage(new Uri(dialog.FileName));
                var image = new Image();
                image.Source = bitmap;

                p.Inlines.Add(image);

                image.Height = 250;

                EditorTextBox.Document.Blocks.Add(p);
            }
            catch (Exception)
            {
                MessageBox.Show("The file you tried to add couldn't be recognised as an image file.");
            }

        }


        #endregion

        private void checkSelection()
        {
            try
            {
                // round to the nearest 0.5
                SelFontSize = Math.Round((double)EditorTextBox.Selection.GetPropertyValue(TextElement.FontSizeProperty) * 2) / 2;
            }
            catch (Exception)
            {

            }

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
            
            try
            {
                var decorations = (TextDecorationCollection)EditorTextBox.Selection.GetPropertyValue(Inline.TextDecorationsProperty);

                // Credit: https://stackoverflow.com/questions/57593990/how-to-check-if-text-in-a-richtextbox-is-underlined

                if (decorations.Count == 0)
                {
                    IsUnderline = false;
                }
                else
                {
                    // must iterate through all possible decorations selected
                    foreach (var decoration in decorations)
                    {
                        if (decoration.Location == TextDecorationLocation.Underline) IsUnderline = true;
                    }
                }
                
            }
            catch (Exception)
            {
                IsUnderline = false;
            }

            try
            {
                var alignment = (TextAlignment)EditorTextBox.Selection.GetPropertyValue(FlowDocument.TextAlignmentProperty);

                switch (alignment)
                {
                    case TextAlignment.Left:
                        selectAlignButton(ToggleAlignLeftButton);
                        break;
                    case TextAlignment.Right:
                        selectAlignButton(ToggleAlignRightButton);
                        break;
                    case TextAlignment.Center:
                        selectAlignButton(ToggleAlignCenterButton);
                        break;
                    case TextAlignment.Justify:
                        selectAlignButton(ToggleAlignJustifyButton);
                        break;
                }
            }
            catch (Exception)
            {
                // don't have any of them toggled
                selectAlignButton(null);
            }

            try
            {
                var blAlignment = (BaselineAlignment)EditorTextBox.Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);
                IsSubscript = blAlignment == BaselineAlignment.Subscript;
                IsSuperscript = blAlignment == BaselineAlignment.Superscript;
            }
            catch (Exception)
            {
                IsSubscript = false;
                IsSuperscript = false;
            }
        }

        private void editorTextBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            checkSelection();
        }


        private void editorTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            checkSelection();
            wasChangedInternally = true;
            RichDocument = EditorTextBox.Document;
        }

        private void editorTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control) {

                // Bold - CTRL + B
                if (e.Key == Key.B)
                {
                    if (EditorTextBox.Selection.Text.Length == 0)
                        checkSelection();
                }

                // Italic - CTRL + I
                if (e.Key == Key.I)
                {
                    if (EditorTextBox.Selection.Text.Length == 0)
                        checkSelection();
                }

                // Underline - CTRL + U
                if (e.Key == Key.U)
                {
                    if (EditorTextBox.Selection.Text.Length == 0)
                        checkSelection();
                }

                // On Paste
                if (e.Key == Key.V)
                {
                    var tr = new TextRange(EditorTextBox.Document.ContentStart, EditorTextBox.Document.ContentEnd);
                    tr.ApplyPropertyValue(FontFamilyProperty, EditorTextBox.FontFamily);
                }

                return;
            }
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

    }
}
