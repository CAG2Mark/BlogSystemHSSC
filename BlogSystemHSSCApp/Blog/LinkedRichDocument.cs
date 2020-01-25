using BlogSystemHSSC.CustomControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BlogSystemHSSC.Blog
{
    /// <summary>
    /// Allows for the reliable display of FlowDocument by assining only one alive RichTextBox to each FlowDocument.
    /// </summary>
    public class LinkedRichDocument : BindableBase
    {
        public LinkedRichDocument(FlowDocument document)
        {
            var rtb = new DisconnectableRtb(document);
            AssignedTextBox = rtb;
            AssignedDocument = document;
        }

        private DisconnectableRtb assignedTextBox;
        private FlowDocument assignedDocument;

        public FlowDocument AssignedDocument
        {
            get
            {
                return assignedDocument;
            }
            set
            {
                Set(ref assignedDocument, value);
                if (AssignedTextBox == null) AssignedTextBox = new DisconnectableRtb(value);
            }
        }
        public DisconnectableRtb AssignedTextBox { get => assignedTextBox; set => Set(ref assignedTextBox, value); }
    }
}
