using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace BlogSystemHSSC.CustomControls
{
    /// <summary>
    /// Extension of the RichTextBox control which contains an event to request to be disconnected.
    /// </summary>
    public class DisconnectableRtb : RichTextBox
    {
        public DisconnectableRtb(FlowDocument doc)
        {
            Document = doc;
            IsDocumentEnabled = true;
            FontSize = 18;
        }


        public event EventHandler RequestDisconnect;
        public void Disconnect()
        {
            RequestDisconnect?.Invoke(this, new EventArgs());
        }
    }
}
