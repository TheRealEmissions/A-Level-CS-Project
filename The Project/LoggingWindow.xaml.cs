using System.Collections.Generic;
using System.Windows;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for LoggingWindow.xaml
    /// </summary>
    public partial class LoggingWindow : Window
    {
        private readonly List<string> OutputLog = new();

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