namespace The_Project
{
    /// <summary>
    /// Interaction logic for ErrorWindow.xaml
    /// </summary>
    public sealed partial class ErrorWindow
    {
        private string _error;

        internal ErrorWindow()
        {
            InitializeComponent();
        }

        internal ErrorWindow SetError(string error)
        {
            _error = error;
            TxtblockError.Text = error;
            return this;
        }

        internal ErrorWindow Initialize()
        {
            Show();
            return this;
        }
    }
}