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

namespace SysCfgTools.SuperBackupUtility
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        private string _text;

        public LogWindow(string text)
        {
            InitializeComponent();
            _text = text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtLog.Text = _text;
        }
    }
}
