using System;
using System.Diagnostics;
using System.Net;

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using The_Project.Accounts;
using The_Project.Database.Tables;
using The_Project.Events;
using The_Project.Exceptions;
using The_Project.Networking;
using The_Project.Networking.Extensions;
using The_Project.Networking.Packets;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for UserConnection.xaml
    /// </summary>
    public sealed partial class UserConnectionPage
    {
        private readonly MainWindow _mainWindow;
        private Listener Listener { get; }
        private RecipientConnection RecipientConnection { get; set; }
        private Tables Tables { get; }
        private readonly SqliteConnection _connection;

        public event EventHandler<ConnectionAcceptedEventArgs> ConnectionAccepted;
        public event EventHandler<ConnectionDeclinedEventArgs> ConnectionDeclined;

        internal UserConnectionPage(MainWindow mainWindow, Tables tables, SqliteConnection connection)
        {
            _mainWindow = mainWindow;
            Tables = tables;
            Listener = new Listener(mainWindow.Handler.UserAccount?.ToUserId() ?? new UserId(), mainWindow,
                mainWindow.DebugWindow);
            _connection = connection;

            InitializeComponent();

            TxtblockUserId.Text = mainWindow.Handler.UserAccount?.ToUserId().Id;
            TxtblockPort.Text += Listener.Port.ToString();
            TxtinputUserid.AddHandler(MouseLeftButtonDownEvent,
                new MouseButtonEventHandler(Txtinput_userid_MouseLeftButtonDown), true);

            ConnectionAccepted += OnConnectionAccepted;
            ConnectionDeclined += OnConnectionDeclined;

            _ = HandleConnection();
        }

        private void OnConnectionDeclined(object sender, ConnectionDeclinedEventArgs e)
        {
            TerminateConnection();
        }

        private void OnConnectionAccepted(object sender, ConnectionAcceptedEventArgs e)
        {
            Debug.WriteLine("Sending connection accepted packet!");
            RecipientConnection.TcpClient?.GetStream().WriteData(new Packet
            {
                Data = new ConnectionVerifiedPacket {A = true, ID = _mainWindow.Handler.UserAccount?.AccountId},
                T = (int) PacketIdentifier.Packet.ConnectionVerified
            });
            Debug.WriteLine("Sent connection accepted packet");
            MessagePage messagePage = new(_mainWindow.Handler.UserAccount?.ToUserId() ?? new UserId(),
                (RecipientConnection?.TcpClient?.GetStream().Socket.RemoteEndPoint as IPEndPoint)?.Address,
                _mainWindow.Handler.Recipient, _mainWindow, Tables, _connection);
            Debug.WriteLine("Launching message page");
            _mainWindow.Content = messagePage;
            if (_mainWindow.Handler.UserAccount is null || _mainWindow.Handler.Recipient is null)
            {
                return;
            }

            Debug.WriteLine("polling starting");
            Listener.Poll(_mainWindow.Handler.UserAccount, _mainWindow.Handler.Recipient, messagePage);
        }

#nullable enable
        private async Task HandleConnection()
        {
            Task<RecipientConnection?> recipientConnection =
                Listener.ListenAndConnect(_mainWindow.Handler.UserAccount?.AccountId ?? "NO_ACCOUNT");
            RecipientConnection = await recipientConnection;
            Debug.WriteLine(RecipientConnection);

            _mainWindow.Handler.Recipient = new Recipient(RecipientConnection);

            // accept/reject connection
            // if accepted, continue
            // if rejected, terminate connection
            ConnectionAcceptWindow connectionAcceptWindow = new(this,
                (RecipientConnection?.TcpClient?.GetStream()
                    .Socket.RemoteEndPoint as IPEndPoint)?.Address ??
                Utils.GetLocalIpAddress(),
                ConnectionAccepted,
                ConnectionDeclined);
            connectionAcceptWindow.Show();
        }

        private void TerminateConnection()
        {
            RecipientConnection.TcpClient?.Close();
            Content = new UserConnectionPage(_mainWindow, Tables, _connection);
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
                RecipientConnection = new RecipientConnection(_mainWindow, _mainWindow.DebugWindow);
                try
                {
                    bool connected = await RecipientConnection.ConnectTo(new UserId(TxtinputUserid.Text));
                    Debug.WriteLine($"FINAL -> Connected to client? {connected}");
                    if (!connected)
                    {
                        Debug.WriteLine("No connection!");
                        return;
                    }

                    _mainWindow.Handler.Recipient = new Recipient(RecipientConnection);
                    Debug.WriteLine("Created new Recipient");

                    MessagePage messagePage = new(_mainWindow.Handler.UserAccount?.ToUserId() ?? new UserId(),
                        (RecipientConnection.TcpClient?.GetStream().Socket.RemoteEndPoint as IPEndPoint)?.Address,
                        _mainWindow.Handler.Recipient, _mainWindow, Tables, _connection);
                    Debug.WriteLine("Created new message page!");
                    _mainWindow.Content = messagePage;
                    Debug.WriteLine("Set main window content with message page");
                    try
                    {
                        if (_mainWindow.Handler.UserAccount is not null && _mainWindow.Handler.Recipient is not null)
                        {
                            await Listener.Poll(_mainWindow.Handler.UserAccount, _mainWindow.Handler.Recipient,
                                messagePage);
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine(exception);
                        Debug.WriteLine(exception.Message);
                        Debug.WriteLine(exception.Data);
                        Debug.WriteLine(exception.StackTrace);
                    }

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

        private void TxtinputUserid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Btn_connect_Click(sender, e);
            }
        }
    }
}