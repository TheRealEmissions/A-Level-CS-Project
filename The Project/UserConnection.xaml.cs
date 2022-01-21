using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using The_Project.Accounts;
using The_Project.Networking;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for UserConnection.xaml
    /// </summary>
    public partial class UserConnectionPage : Page
    {
        private readonly MainWindow MainWindow;

        public UserConnectionPage(MainWindow MainWindow)
        {
            this.MainWindow = MainWindow;
            InitializeComponent();

            txtblock_userId.Text = MainWindow.Handler.UserAccount.ToUserId().Id;

            Listener Listener = new(MainWindow.Handler.UserAccount.ToUserId(), MainWindow.DebugWindow);
            Task<RecipientConnection> RecipientConnection = Listener.ListenAndConnect(MainWindow.Handler.UserAccount.AccountId);

            txtinput_userid.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_userid_MouseLeftButtonDown), true);
        }

        private void Txtinput_userid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (txtinput_userid.Text is "Input User ID")
            {
                txtinput_userid.Text = string.Empty;
            }
        }

        private void Txtinput_userid_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Btn_debugWindow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Btn_DebugWindow_Click(sender, e);
        }

        private void Btn_connect_Click(object sender, RoutedEventArgs e)
        {
            if (new UserId().Regex.IsMatch(txtinput_userid.Text))
            {
                // connect to user
            }
            else
            {
                return;
            }
        }
    }
}