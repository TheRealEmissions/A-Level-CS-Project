using System;
using System.Net;
using System.Windows.Controls;
using The_Project.Accounts;
using The_Project.Cryptography;
using The_Project.Events;

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

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        internal MessagePage(UserId selfUserId, IPAddress recipientIpAddress, Recipient recipient,
            MainWindow mainWindow)
        {
            _selfUserId = selfUserId;
            _recipientIpAddress = recipientIpAddress;
            _recipient = recipient;
            _mainWindow = mainWindow;

            MessageReceived += MessageReceivedFromRecipient;

            InitializeComponent();

            TxtblockUser.Text = recipientIpAddress.ToString();
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


        private void BtnTerminateConnection_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _recipient.Connection.TcpClient?.Close();
            _recipient.Connection.TcpClient = null;
            Content = new UserConnectionPage(_mainWindow).Content;
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
        }

        private void MessageReceivedFromRecipient(object sender, MessageReceivedEventArgs e)
        {
            // update db
            // update message list
            UpdateMessagesList(e.Ciphertext.Decrypt(_mainWindow.EncryptionKeys.PrivateKey), true);
        }
    }
}