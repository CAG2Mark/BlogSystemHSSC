using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BlogSystemHSSC.CustomControls
{
    public class BlogImage : Image
    {
        private string fileName;
        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                this.Source = new BitmapImage(new Uri(Global.ViewModel.FilesPath + "\\Images\\" + value));
            }
        }

        public BlogImage(string name)
        {
            this.FileName = name;
            this.MaxWidth = 500;
            this.Width = Double.NaN;
        }

        public BlogImage()
        {
            this.MaxWidth = 500;
            this.Width = Double.NaN;
        }

        public void ForceUpdate()
        {
            this.Source = new BitmapImage(new Uri(Global.ViewModel.FilesPath + "\\Images\\" + FileName));
        }

        
    }
}
