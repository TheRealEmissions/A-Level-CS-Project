using System;

using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Microsoft.Data.Sqlite;
using The_Project.Accounts;
using The_Project.Cryptography;
using The_Project.Database;
using The_Project.Database.Tables;
using The_Project.Events;
using The_Project.Networking;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for MessagePage.xaml
    /// </summary>
    public sealed partial class MessagePage
    {
        private UserId _selfUserId;
        private IPAddress _recipientIpAddress;
        private readonly Recipient _recipient;
        private readonly MainWindow _mainWindow;
        private readonly Tables _tables;
        private readonly SqliteConnection _connection;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        internal MessagePage(UserId selfUserId, IPAddress recipientIpAddress, Recipient recipient,
            MainWindow mainWindow, Tables tables, SqliteConnection connection)
        {
            _selfUserId = selfUserId;
            _recipientIpAddress = recipientIpAddress;
            _recipient = recipient;
            _mainWindow = mainWindow;
            _tables = tables;
            _connection = connection;
            
            MessageReceived += MessageReceivedFromRecipient;

            InitializeComponent();

            TxtblockUser.Text = recipientIpAddress.ToString();

            TxtMsgContent.AddHandler(MouseLeftButtonDownEvent, new MouseButtonEventHandler(TxtMsgContent_OnMouseDown));
        }

        internal void OnMessageReceived(MessageReceivedEventArgs e)
        {
            MessageReceived?.Invoke(this, e);
        }

        private void UpdateMessagesList(string text, bool received)
        {
            ListBoxItem listBoxItem = new()
            {
                Content = !received
                    ? $"Me: {text}"
                    : $"{_recipient.Nickname ?? "Them"}: {text}"
            };
            _ = ListboxMessages.Items.Add(listBoxItem);
        }

        private void StoreMessageInDatabase(string text, bool received)
        {
            Database.UserAccount userAccount = new(_tables);
            userAccount.AddMessage(new Messages.MessageSchema(_mainWindow.Handler.UserAccount?.AccountId,
                _recipient.AccountId,
                (int) DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                text, received));
        }


        private void BtnTerminateConnection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _recipient.Connection.TcpClient?.Close();
            _recipient.Connection.TcpClient = null;
            _recipient.Nickname = null;
            _recipient.PublicKeyStored = false;
            _recipient.Connection = new RecipientConnection(_mainWindow, _mainWindow.DebugWindow);
            _recipient.PublicKey = new PublicKey();
            _recipient.AccountId = null;

            _mainWindow.Content = new UserConnectionPage(_mainWindow, _tables, _connection);
        }

        private void BtnSend_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (TxtMsgContent.Text.Length <= 0)
            {
                return;
            }

            if (TxtMsgContent.Text == "Message Content")
            {
                return;
            }

            _recipient.Send(TxtMsgContent.Text);
            UpdateMessagesList(TxtMsgContent.Text, false);
            StoreMessageInDatabase(TxtMsgContent.Text, false);

            TxtMsgContent.Text = string.Empty;
        }

        private void MessageReceivedFromRecipient(object sender, MessageReceivedEventArgs e)
        {
            string text = e.Ciphertext.Decrypt(_mainWindow.EncryptionKeys.PrivateKey);
            UpdateMessagesList(text, true);
            StoreMessageInDatabase(text, true);
        }

        private void TxtMsgContent_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnSend_Click(sender, e);
            }
        }

        private void TxtMsgContent_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (TxtMsgContent.Text == "Message Content")
            {
                return;
            }

            TxtMsgContent.Text = string.Empty;
        }

        private void BtnNickname_Click(object sender, RoutedEventArgs e)
        {
            if (TxtNickname.Text == "Nickname")
            {
                return;
            }

            _recipient.Nickname = TxtNickname.Text;
            TxtblockUser.Text = TxtNickname.Text;

            Database.RecipientAccount recipientAccount =
                new(_connection, _mainWindow.Handler.UserAccount, _tables);
            recipientAccount.UpdateNickname(TxtNickname.Text, _recipient.AccountId);

            TxtNickname.Text = "Nickname";

        }
    }
}