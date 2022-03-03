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
        private Listener Listener { get; init; }
        protected RecipientConnection RecipientConnection { get; private set; }

        public UserConnectionPage(MainWindow MainWindow)
        {
            this.MainWindow = MainWindow;
            this.Listener = new(MainWindow.Handler.UserAccount.ToUserId(), MainWindow.DebugWindow);
            InitializeComponent();

            txtblock_userId.Text = MainWindow.Handler.UserAccount.ToUserId().Id;
            txtblock_port.Text += Listener.Port.ToString();
            txtinput_userid.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_userid_MouseLeftButtonDown), true);

            _ = HandleConnection();
        }

        public async Task HandleConnection()
        {
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

            return;
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

        private async void Btn_connect_Click(object sender, RoutedEventArgs e)
        {
            btn_connect.IsEnabled = false;
            MainWindow.Debug($"Connecting to {txtinput_userid.Text}");
            if (UserId.Regex.Match(txtinput_userid.Text).Success)
            {
                MainWindow.Debug("Regex checks out! Processing connection.");
                RecipientConnection = new RecipientConnection(MainWindow.DebugWindow);
                try
                {
                    bool Connected = await RecipientConnection.ConnectTo(new UserId(txtinput_userid.Text));
                    /*                    if (!Connected)
                                        {
                                            throw new ConnectionRefusedException("Could not connect!");
                                        }*/
                }
                catch (ConnectionRefusedException)
                {
                    btn_connect.IsEnabled = true;
                    throw;
                }
            }
            else
            {
                MainWindow.Debug("Regex failed validation.");
                return;
            }
        }
    }
}