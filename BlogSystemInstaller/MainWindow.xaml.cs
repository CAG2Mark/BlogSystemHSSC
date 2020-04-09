using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;
using BlogSystemInstaller;

namespace BlogSystemInstaller
{
    // NOTE: Most of the code in this file is copied from my previous projects.

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LicenseTextBox.Text = Properties.Resources.BlogLicense;

            if (File.Exists(exeDirec)) InstallingUpdateCheckBox.IsChecked = true;
        }

        const string installDirec = @"C:\Program Files\Blog System";
        const string zipDirec = installDirec + @"\Libraries.zip";
        const string icoDirec = installDirec + @"\icon.ico";
        const string exeDirec = installDirec + @"\Blog System.exe";
        const string uninstallDirec = installDirec + @"\uninstall.exe";

        private void InstallClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Directory.CreateDirectory(installDirec);

                File.WriteAllBytes(exeDirec, Properties.Resources.BlogSystemHSSC);
                File.WriteAllBytes(uninstallDirec, Properties.Resources.BlogSystemUninstaller);

                using (FileStream fileStream = new FileStream(icoDirec, FileMode.Create)) //FileStream writes the icon file as WriteAllBytes does not work for icons
                {
                    Properties.Resources.icon.Save(fileStream);
                }

                setRegistry();
                setShortcuts();

                if (InstallingUpdateCheckBox.IsChecked == false)
                {

                    File.WriteAllBytes(zipDirec, Properties.Resources.Libraries);
                    ZipFile.ExtractToDirectory(zipDirec, installDirec);

                }

                // install success

                MasterTabControl.SelectedIndex = 1;
                
            }
            catch (Exception)
            {
                MessageBox.Show("Install failed - try disabling your antivirus temporarily.");
            }
        }

        private void setRegistry()
        {
            string registryDirec = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            string appName = "Blog System";

            RegistryKey HKEY = (Registry.LocalMachine).OpenSubKey(registryDirec, true); //Declares the HKEY directory in the registry
            RegistryKey applicationKey = HKEY.CreateSubKey(appName); //Creates the "Blog System" key under the HKEY variable

            //Syntax: SetValue(Name, Value, Type)
            applicationKey.SetValue("DisplayName", appName, RegistryValueKind.String);
            applicationKey.SetValue("DisplayVersion", "1.0.0.0", RegistryValueKind.String);
            applicationKey.SetValue("InstallDate", DateTime.Now.ToString("yyyyMMdd"), RegistryValueKind.String);
            applicationKey.SetValue("InstallLocation", exeDirec, RegistryValueKind.String);
            applicationKey.SetValue("UninstallString", uninstallDirec, RegistryValueKind.String);
        }

        private void setShortcuts()
        {
            IWshRuntimeLibrary.WshShell wshShell = new IWshRuntimeLibrary.WshShell();

            string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            IWshRuntimeLibrary.IWshShortcut desktopShortCut = default(IWshRuntimeLibrary.IWshShortcut);
            desktopShortCut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(desktopFolder + "\\Blog System.lnk");
            desktopShortCut.TargetPath = exeDirec;
            desktopShortCut.WorkingDirectory = installDirec;
            desktopShortCut.Save();

            string startFolder = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            IWshRuntimeLibrary.IWshShortcut startMenuShortCut = default(IWshRuntimeLibrary.IWshShortcut);
            startMenuShortCut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(startFolder + "\\Blog System.lnk");
            startMenuShortCut.TargetPath = exeDirec;
            startMenuShortCut.WorkingDirectory = installDirec;
            startMenuShortCut.Save();
        }

        private void Launch(object sender, RoutedEventArgs e)
        {
            Process.Start(exeDirec);
            Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
