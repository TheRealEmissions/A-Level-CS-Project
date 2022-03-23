using System.Windows;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public partial class ErrorWindow
    {
        private string _error;

        public ErrorWindow()
        {
            InitializeComponent();
        }

        public ErrorWindow SetError(string error)
        {
            this._error = error;
            TxtblockError.Text = error;
            return this;
        }

        public ErrorWindow Initialize()
        {
            this.Show();
            return this;
        }
    }
}