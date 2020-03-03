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
    /// Interaction logic for EmbedContainer.xaml
    /// </summary>
    public partial class EmbedContainer : UserControl, INotifyPropertyChanged
    {

        public static readonly DependencyProperty IFrameCodeProperty = DependencyProperty.Register(
            "IFrameCode", 
            typeof(string),
            typeof(EmbedContainer),
            new PropertyMetadata("", 
                (o, e) => ((EmbedContainer)o).IFrameCodeChanged()
            )
        );

        public string IFrameCode
        {
            get => (string)GetValue(IFrameCodeProperty);
            set => SetValue(IFrameCodeProperty, value);
        }

        private void IFrameCodeChanged()
        {
            OnPropertyChanged(nameof(IFrameCode));
        }

        public EmbedContainer()
        {
            InitializeComponent();
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invokes PropertyChanged on a given propertyName.
        /// </summary>
        /// <param name="propertyName">The name of the property on which to call the PropertyChanged event on.</param>
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
