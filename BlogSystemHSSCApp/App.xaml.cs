using CefSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace BlogSystemHSSC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Improves RichTextBox input performance but decreases animation performance. Must override the default value.
            Timeline.DesiredFrameRateProperty.OverrideMetadata(typeof(Timeline), new FrameworkPropertyMetadata { DefaultValue = 1 });
        }

        public App()
        {
            Exit += onAppExit;
        }

        private void onAppExit(object sender, ExitEventArgs e)
        {
            Cef.Shutdown();
        }
    }
}
