using System.Diagnostics;
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
    public partial class UserConnectionPage
    {
        private readonly MainWindow _mainWindow;
        private Listener Listener { get; }
        protected RecipientConnection RecipientConnection { get; private set; }

        public UserConnectionPage(MainWindow mainWindow)
        {
            this._mainWindow = mainWindow;
            this.Listener = new(mainWindow.Handler.UserAccount.ToUserId(), mainWindow.DebugWindow);
            InitializeComponent();

            TxtblockUserId.Text = mainWindow.Handler.UserAccount.ToUserId().Id;
            TxtblockPort.Text += Listener.Port.ToString();
            TxtinputUserid.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(Txtinput_userid_MouseLeftButtonDown), true);

            _ = HandleConnection();
        }

        public async Task HandleConnection()
        {
            Task<RecipientConnection> recipientConnection = Listener.ListenAndConnect(_mainWindow.Handler.UserAccount.AccountId);
            this.RecipientConnection = await recipientConnection;

            // accept/reject connection
            // if accepted, continue
            // if rejected, terminate connection
            try
            {
                ConnectionAcceptWindow connectionAcceptWindow = new(this, ((IPEndPoint)recipientConnection.Result.TcpClient.GetStream().Socket.RemoteEndPoint).Address);
            }
            catch (RejectConnectionException)
            {
                this.Content = _mainWindow;
            }

        }

        public void TerminateConnection()
        {
            RecipientConnection.TcpClient.Close();
            this.Content = new UserConnectionPage(_mainWindow);
        }

        private void Txtinput_userid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TxtinputUserid.Text is "Input User ID")
            {
                TxtinputUserid.Text = string.Empty;
            }
        }

        private void Txtinput_userid_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Btn_debugWindow_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.Btn_DebugWindow_Click(sender, e);
        }

        private async void Btn_connect_Click(object sender, RoutedEventArgs e)
        {
            BtnConnect.IsEnabled = false;
            _mainWindow.Debug($"Connecting to {TxtinputUserid.Text}");
            if (UserId.Regex.Match(TxtinputUserid.Text).Success)
            {
                _mainWindow.Debug("Regex checks out! Processing connection.");
                RecipientConnection = new RecipientConnection(_mainWindow.DebugWindow);
                try
                {
                    bool connected = await RecipientConnection.ConnectTo(new UserId(TxtinputUserid.Text));
                    Debug.WriteLine($"Connected? {connected}");
                    /*                    if (!Connected)
                                        {
                                            throw new ConnectionRefusedException("Could not connect!");
                                        }*/
                }
                catch (ConnectionRefusedException)
                {
                    BtnConnect.IsEnabled = true;
                    throw;
                }
            }
            else
            {
                _mainWindow.Debug("Regex failed validation.");
            }
        }
    }
}