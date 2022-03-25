using System.Collections.Generic;
using System.Windows;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for LoggingWindow.xaml
    /// </summary>
    public partial class LoggingWindow
    {
        private readonly List<string> _outputLog = new();

        public LoggingWindow()
        {
            InitializeComponent();
        }

        public void Debug(string text)
        {
            _outputLog.Add(text);
            if (_outputLog.Count > 100)
            {
                _outputLog.RemoveAt(0);
            }

            TxtLog.Text = string.Join("\n", _outputLog);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }
    }
}