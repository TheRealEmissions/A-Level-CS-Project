﻿using System;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using The_Project.Accounts;
using The_Project.Events;
using The_Project.Exceptions;
using The_Project.Networking;
using The_Project.Networking.Packets;

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

        public event EventHandler<ConnectionAcceptedEventArgs> ConnectionAccepted;
        public event EventHandler<ConnectionDeclinedEventArgs> ConnectionDeclined;

        public UserConnectionPage(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            Listener = new Listener(mainWindow.Handler.UserAccount?.ToUserId() ?? new UserId(), mainWindow,
                mainWindow.DebugWindow);

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
            RecipientConnection.TcpClient?.GetStream().Write(JsonSerializer.SerializeToUtf8Bytes(new Packet
            {
                Data = new ConnectionVerifiedPacket {A = true}, T = (int) PacketIdentifier.Packet.ConnectionVerified
            }));
            MessagePage messagePage = new(_mainWindow.Handler.UserAccount?.ToUserId() ?? new UserId(),
                (RecipientConnection?.TcpClient?.GetStream().Socket.RemoteEndPoint as IPEndPoint)?.Address,
                _mainWindow.Handler.Recipient, _mainWindow);
            _mainWindow.Content = messagePage;
            if (_mainWindow.Handler.UserAccount is not null && _mainWindow.Handler.Recipient is not null)
                _ = Listener.Poll(_mainWindow.Handler.UserAccount, _mainWindow.Handler.Recipient, messagePage);
        }

#nullable enable
        public async Task HandleConnection()
        {
            Task<RecipientConnection?> recipientConnection =
                Listener.ListenAndConnect(_mainWindow.Handler.UserAccount?.AccountId ?? "NO_ACCOUNT");
            RecipientConnection = await recipientConnection;

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
        }

        public void TerminateConnection()
        {
            RecipientConnection.TcpClient?.Close();
            Content = new UserConnectionPage(_mainWindow);
        }

        private void Txtinput_userid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (TxtinputUserid.Text is "Input User ID") TxtinputUserid.Text = string.Empty;
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
                    if (!connected) return;

                    _mainWindow.Handler.Recipient = new Recipient(RecipientConnection);

                    MessagePage messagePage = new(_mainWindow.Handler.UserAccount?.ToUserId() ?? new UserId(),
                        (RecipientConnection.TcpClient?.GetStream().Socket.RemoteEndPoint as IPEndPoint)?.Address,
                        _mainWindow.Handler.Recipient, _mainWindow);
                    await Listener.Poll(_mainWindow.Handler.UserAccount, _mainWindow.Handler.Recipient, messagePage);
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