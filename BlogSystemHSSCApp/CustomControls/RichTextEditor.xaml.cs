using BlogSystemHSSC.Blog;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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
                typeof(LinkedRichDocument), 
                typeof(RichTextEditor),
                new PropertyMetadata(
                    null, 
                    (o, e) => ((RichTextEditor)o).richDocumentChanged()
                    )
                );

        public LinkedRichDocument RichDocument
        {
            get => (LinkedRichDocument)GetValue(RichDocumentProperty);
            set => SetValue(RichDocumentProperty, value);
        }

        public DisconnectableRtb EditorTextBox;

        public string PostUId { get; set; }

        private bool disconnectInvokedFromThis = false;

        bool wasChangedInternally = false;
        private void richDocumentChanged()
        {
            OnPropertyChanged(nameof(RichDocument));

            // prevent infinite loop of setting then callback
            if (!wasChangedInternally)
            {
                if (RichDocument == null) return;

                EditorTextBox = RichDocument.AssignedTextBox;
                EditorTextBox.RequestDisconnect += EditorTextBox_RequestDisconnect;

                EditorTextBox.IsDocumentEnabled = true;

                setChild();
            }
            else wasChangedInternally = false;
        }



        private void EditorTextBox_RequestDisconnect(object sender, EventArgs e)
        {
            // Prevent self-disconnect
            if (!disconnectInvokedFromThis)
            {
                // Deassociate self with the text box
                EditorTextBox.TextChanged -= editorTextBoxTextChanged;
                EditorTextBox.SelectionChanged -= editorTextBoxSelectionChanged;
                EditorTextBox.KeyUp -= editorTextBoxKeyUp;
                EditorTextBox.LostFocus -= editorTextBoxLostFocus;

                DataObject.RemovePastingHandler(EditorTextBox, onPaste);

                RtbContainer.Child = null;
            }
            disconnectInvokedFromThis = false;
        }

        public void setChild()
        {
            // Disconnect editortextbox from its previous parent.
            disconnectInvokedFromThis = true;
            EditorTextBox.Disconnect();

            // Set this as its parent.
            RtbContainer.Child = EditorTextBox;

            // Add event handlers
            EditorTextBox.TextChanged += editorTextBoxTextChanged;
            EditorTextBox.SelectionChanged += editorTextBoxSelectionChanged;
            EditorTextBox.KeyUp += editorTextBoxKeyUp;
            EditorTextBox.LostFocus += editorTextBoxLostFocus;

            DataObject.AddPastingHandler(EditorTextBox, onPaste);

        }

        private void editorTextBoxLostFocus(object sender, RoutedEventArgs e)
        {
            // Make sure all the images are saved!
            saveTextboxImages(EditorTextBox.Document);
        }

        #endregion

        /// <summary>
        /// Provides an interface for the user to easily edit rich text.
        /// </summary>
        public RichTextEditor()
        {
            InitializeComponent();

            //MessageBox.Show("test");
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

            EditorTextBox.Focus();

            if (dialog.ShowDialog() == false) return;

            // Credit: https://social.msdn.microsoft.com/Forums/vstudio/en-US/e4b322c2-2553-4802-bd0d-fe7ef7e90b37/inserting-image-into-richtextbox-control?forum=wpf

            Paragraph p = new Paragraph();

            try
            {
                
                var src = $"{imagePath}\\{PostUId}_{Path.GetFileName(dialog.FileName)}";

                File.Copy(dialog.FileName, src);

                var bitmap = new BitmapImage(new Uri(src));
                var image = new Image();
                image.Source = bitmap;

                p.Inlines.Add(image);

                image.MaxWidth = 500;

                // You have to use a try catch here as PreviousBlock.get returning null throws an exception even if it is never referenced
                try
                {
                    RichDocument.AssignedDocument.Blocks.InsertAfter(EditorTextBox.CaretPosition.Paragraph.PreviousBlock, p);
                }
                catch (Exception)
                {
                    // This is when the blog post is empty and the user wants to add an image
                    RichDocument.AssignedDocument.Blocks.Add(p);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("The file you tried to add couldn't be recognised as an image file.");
            }

        }

        // Credit: https://stackoverflow.com/questions/9279061/dynamically-adding-hyperlinks-to-a-richtextbox

        private void addHyperlinkButtonClicked(object sender, RoutedEventArgs e)
        {

            EditorTextBox.IsDocumentEnabled = true;

            if (EditorTextBox.Selection.IsEmpty)
            {
                MessageBox.Show("Please select some text to add a hyperlink.");
                return;
            }

            TextRange tr = EditorTextBox.Selection;

            // enter link

            EnterUrlDialog d;
            if (isUrl(tr.Text)) d = new EnterUrlDialog(tr.Text);
            else d = new EnterUrlDialog();

            if (d.ShowDialog() != true) return;
            string URI = d.Url;

            Hyperlink link = new Hyperlink(tr.Start, tr.End);

            link.IsEnabled = true;

            try
            {
                link.NavigateUri = new Uri(URI);
            }
            catch
            {
                try
                {
                    link.NavigateUri = new Uri("https://" + URI);
                }
                catch (Exception)
                {
                    MessageBox.Show("Link was in an incorrect format.");
                    return;
                }
            }
            link.RequestNavigate += new RequestNavigateEventHandler(link_RequestNavigate);
        }

        private bool isUrl(string text)
        {
            // Regex credit: https://urlregex.com/

            return Regex.IsMatch(text,
                @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$");
        }

        private void link_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri.ToString());
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
        }

        private void saveTextboxImages(FlowDocument doc)
        {
            foreach (var block in doc.Blocks)
            {
                if (block.GetType() == typeof(BlockUIContainer))
                {
                    var image = (Image)((BlockUIContainer)block).Child;
                    savePastedImage(image);
                }

                if (block.GetType() == typeof(Paragraph))
                {
                    var inlines = ((Paragraph)block).Inlines;
                    foreach (var inline in inlines)
                    {
                        if (inline.GetType() == typeof(InlineUIContainer))
                        {
                            var image = (Image)((InlineUIContainer)inline).Child;
                            savePastedImage(image);
                        }
                    }
                }
            }
        }

        private readonly string imagePath = $"{Global.FilesPath}\\Images";

        private void savePastedImage(Image image)
        {
            image.MaxWidth = 500;

            var sourceStr = image.Source.ToString();

            // When an image is pasted it starts with pack, so this is when it needs to be saved
            if (!sourceStr.StartsWith("pack")) return;

            var fileName = imagePath + $"\\{PostUId}_{Path.GetFileNameWithoutExtension(sourceStr)}.png";

            // Convert image to byte array to save
            var bitmap = (BitmapImage)image.Source;

            byte[] b;

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                b = ms.ToArray();
            }



            // Create the directory if it does not exist
            if (!Directory.Exists(imagePath)) Directory.CreateDirectory(imagePath);

            // Write the file
            File.WriteAllBytes(fileName, b);

            image.Source = new BitmapImage(new Uri(fileName));
        }

        private async void onPaste(object sender, DataObjectPastingEventArgs e)
        {
            // Only perform the task once the data is in the textbox
            await Task.Delay(1);

            var tr = new TextRange(EditorTextBox.Document.ContentStart, EditorTextBox.Document.ContentEnd);
            tr.ApplyPropertyValue(FontFamilyProperty, EditorTextBox.FontFamily);
            tr.ApplyPropertyValue(FontSizeProperty, EditorTextBox.FontSize);

            // Save all pasted images

            saveTextboxImages(EditorTextBox.Document);
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
