using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for LoggingWindow.xaml
    /// </summary>
    public partial class LoggingWindow : Window
    {
        public List<string> OutputLog = new();

        public LoggingWindow()
        {
            InitializeComponent();
        }

        public void Debug(string text)
        {
            OutputLog.Add(text);
            if (OutputLog.Count > 100)
            {
                OutputLog.RemoveAt(0);
            }

            txt_log.Text = string.Join("\n", OutputLog);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }
}