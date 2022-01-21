using System.Windows;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow : Window
    {
        private string Error;

        public ErrorWindow()
        {
            InitializeComponent();
        }

        public ErrorWindow SetError(string Error)
        {
            this.Error = Error;
            txtblock_error.Text = Error;
            return this;
        }

        public ErrorWindow Initialize()
        {
            this.Show();
            return this;
        }
    }
}