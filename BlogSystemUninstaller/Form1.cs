using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlogSystemUninstaller
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Uninstall();
        }

        const string installDirec = @"C:\Program Files\Blog System";

        private void Uninstall()
        {
            // 1) run script to delete
            // 2) un registry
            // 3) delete shortcuts

            deleteFolderContents(new DirectoryInfo(installDirec));
            deleteRegistry();
            deleteShortcuts();

            Close();
        }

        // deletes files in a folder individually so that files in use do not stop the whole delete process, whcih can happen with DirectoryInfo.Delete()
        private static void deleteFolderContents(DirectoryInfo direc)
        {
            foreach (var file in direc.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception)
                {

                }
            }
            foreach (var subDirec in direc.GetDirectories())
            {
                deleteFolderContents(subDirec);
                subDirec.Delete();
            }
            
        }

        private void deleteRegistry()
        {
            string registryDirec = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
            string appName = "Blog System";

            RegistryKey HKEY = (Registry.LocalMachine).OpenSubKey(registryDirec, true); //Declares the HKEY directory in the registry
            RegistryKey applicationKey = HKEY.OpenSubKey(appName, true); //Creates the "FlyLive Studio" key under the HKEY variable

            if (applicationKey != null) HKEY.DeleteSubKey(appName);
        }

        private void deleteShortcuts()
        {
            string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (File.Exists(desktopFolder + "\\Blog System.lnk")) File.Delete(desktopFolder + "\\Blog System.lnk");

            string startFolder = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
            if (File.Exists(startFolder + "\\Blog System.lnk")) File.Delete(startFolder + "\\Blog System.lnk");

        }
    }
}
