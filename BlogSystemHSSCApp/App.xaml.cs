using CefSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BlogSystemHSSC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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
