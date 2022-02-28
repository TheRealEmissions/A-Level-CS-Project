using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using The_Project.Accounts;
using The_Project.Exceptions;
using The_Project.Networking;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for UserConnection.xaml
    /// </summary>
    public partial class UserConnectionPage : Page
    {
        private readonly MainWindow MainWindow;
        protected RecipientConnection RecipientConnection { get; private set; }

        public UserConnectionPage(MainWindow MainWindow)
        {
            this.MainWindow = MainWindow;
            InitializeComponent();

            txtblock_userId.Text = MainWindow.Handler.UserAccount.ToUserId().Id;
            txtinput_userid.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_userid_MouseLeftButtonDown), true);

            HandleConnection();
        }

        public async void HandleConnection()
        {
            Listener Listener = new(MainWindow.Handler.UserAccount.ToUserId(), MainWindow.DebugWindow);
            Task<RecipientConnection> RecipientConnection = Listener.ListenAndConnect(MainWindow.Handler.UserAccount.AccountId);
            this.RecipientConnection = await RecipientConnection;

            // accept/reject connection
            // if accepted, continue
            // if rejected, terminate connection
            try
            {
                ConnectionAcceptWindow ConnectionAcceptWindow = new(this, ((IPEndPoint)RecipientConnection.Result.Client.GetStream().Socket.RemoteEndPoint).Address);
            }
            catch (RejectConnectionException)
            {
                this.Content = MainWindow;
            }
        }

        public void TerminateConnection()
        {
            RecipientConnection.Client.Close();
            this.Content = new UserConnectionPage(MainWindow);
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
            MainWindow.Debug($"Connecting to {txtinput_userid.Text}");
            if (UserId.Regex.Match(txtinput_userid.Text).Success)
            {
                MainWindow.Debug("Regex checks out! Processing connection.");
                this.RecipientConnection = new RecipientConnection();
                bool Connected = RecipientConnection.ConnectTo(new UserId(txtinput_userid.Text));
                if (!Connected) { throw new ConnectionRefusedException("CONNECTION REFUSED"); }
            }
            else
            {
                MainWindow.Debug("Regex failed validation.");
                return;
            }
        }
    }
}