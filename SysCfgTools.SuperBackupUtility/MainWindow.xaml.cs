using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using SysCfgTools.SuperBackupUtility.Properties;
using Microsoft.Win32;
using System.Diagnostics;
using SysCfgTools.UtilLib;

namespace SysCfgTools.SuperBackupUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Utils.RestoreComplete += new Action(Utils_RestoreComplete);
            Utils.BackupCompleted += new Action(Utils_BackupCompleted);
        }

        void Utils_BackupCompleted()
        {
            EnableControls();
        }

        void Utils_RestoreComplete()
        {
            EnableControls();
        }
  
        /// <summary>
        /// Disables all the controls on the form
        /// </summary>
        private void DisableControls()
        {
            gridControlGrid.IsEnabled = false;
        }

        /// <summary>
        /// Enable all the controls on the window
        /// </summary>
        private void EnableControls()
        {
            //*might* not be called on our thread, so give it to the dispatcher
            Dispatcher.Invoke((Action)delegate
            {
                gridControlGrid.IsEnabled = true;
            });
        }
    
        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (FindUtils())
            {
                try
                {
                    MessageBoxResult res = MessageBox.Show("This will overwrite all data in the database, ok to continue?", "Sanity Check", MessageBoxButton.YesNo);
                    if (res == MessageBoxResult.No)
                        return;
                    DisableControls();
                    UtilLib.Utils.RestoreFile(Settings.Default.BashPath, Settings.Default.BackupScriptPath, txtFile.Text);                    
                }
                catch (Exception ex)
                {
                    EnableControls();
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Could not find necessary components needed for this operation, aborting.");
            }
        }

        private void btnBackup_Click(object sender, RoutedEventArgs e)
        {
            //find the script and bash
            if (FindUtils())
            {
                try
                {
                    if (File.Exists(txtFile.Text))
                    {
                        MessageBoxResult res = MessageBox.Show("File exists, are you sure you want to overwrite?", "Sanity Check", MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.No)
                            return;
                    }
                    DisableControls();
                    UtilLib.Utils.BackupFile(Settings.Default.BashPath, Settings.Default.BackupScriptPath, txtFile.Text, (bool)chkBackwardsCompat.IsChecked);
                }
                catch (Exception ex)
                {
                    EnableControls();
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("Could not find necessary components needed for this operation, aborting.");
            }
        }

        private bool FindUtils()
        {
            if (!File.Exists(Settings.Default.BashPath))
            {
                if (File.Exists("C:\\Program Files\\Foxboro\\cygwin\\bin\\bash.exe"))
                {
                    Settings.Default.BashPath = "C:\\Program Files\\Foxboro\\cygwin\\bin\\bash.exe";
                    Settings.Default.Save();
                }
                else if (File.Exists("C:\\Program Files (x86)\\Foxboro\\cygwin\\bin\\bash.exe"))
                {
                    Settings.Default.BashPath = "C:\\Program Files (x86)\\Foxboro\\cygwin\\bin\\bash.exe";
                    Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("Can't find bash.exe, please show me:");
                    OpenFileDialog d = new OpenFileDialog();
                    d.Filter = "bash.exe|bash.exe";
                    if ((bool)d.ShowDialog())
                    {
                        if (!File.Exists(d.FileName))
                        {
                            MessageBox.Show("File doesn't exist, aborting");
                            return false;
                        }
                        Settings.Default.BashPath = d.FileName;
                        Settings.Default.Save();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (!File.Exists(Settings.Default.BackupScriptPath))
            {
                if (File.Exists("C:\\Program Files\\Foxboro\\System Configurator\\scripts\\database_maintenance.sh"))
                {
                    Settings.Default.BackupScriptPath = "C:\\Program Files\\Foxboro\\System Configurator\\scripts\\database_maintenance.sh";
                    Settings.Default.Save();
                }
                else if (File.Exists("C:\\Program Files (x86)\\Foxboro\\System Configurator\\scripts\\database_maintenance.sh"))
                {
                    Settings.Default.BackupScriptPath = "C:\\Program Files (x86)\\Foxboro\\System Configurator\\scripts\\database_maintenance.sh";
                    Settings.Default.Save();
                }
                else
                {
                    MessageBox.Show("Can't find database_maintenance.sh, please show me:");
                    OpenFileDialog d = new OpenFileDialog();
                    d.Filter = "database_maintenance.sh|database_maintenance.sh";
                    if ((bool)d.ShowDialog())
                    {
                        if (!File.Exists(d.FileName))
                        {
                            MessageBox.Show("File doesn't exist, aborting");
                            return false;
                        }
                        Settings.Default.BackupScriptPath = d.FileName;
                        Settings.Default.Save();
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void btnBroswe_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog d = new OpenFileDialog();
            d.CheckFileExists = false;
            if ((bool)d.ShowDialog())
            {
                txtFile.Text = d.FileName;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //disable this button on xp as it doesnt apply
            OperatingSystem os = Environment.OSVersion;
            if (os.Version.Major < 6)
            {
                chkBackwardsCompat.IsEnabled = false;
            }
        }

        private void btnShowLog_Click(object sender, RoutedEventArgs e)
        {
            LogWindow lw = new LogWindow(UtilLib.Utils.StandardOutput);
            lw.ShowDialog();
        }


    }
}
