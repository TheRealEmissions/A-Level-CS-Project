using System;
using System.Net;
using System.Windows;
using The_Project.Events;
using The_Project.Exceptions;

#nullable enable
namespace The_Project
{
    /// <summary>
    /// Interaction logic for ConnectionAccept.xaml
    /// </summary>
    public sealed partial class ConnectionAcceptWindow
    {
        private readonly UserConnectionPage _userConnectionWindow;

        private readonly EventHandler<ConnectionAcceptedEventArgs> _connectionAccepted;
        private readonly EventHandler<ConnectionDeclinedEventArgs> _connectionDeclined;

        internal ConnectionAcceptWindow(UserConnectionPage userConnectionWindow, IPAddress? ipAddress,
            EventHandler<ConnectionAcceptedEventArgs> connectionAcceptedEventHandler,
            EventHandler<ConnectionDeclinedEventArgs> connectionDeclinedEventHandler)
        {
            /*if (ipAddress is null)
            {
                throw new CreateConnectionException("ip address not found for recipient");
            }*/

            InitializeComponent();

            _userConnectionWindow = userConnectionWindow;
            TxtblockIpAddress.Text = ipAddress?.ToString();
            _connectionAccepted = connectionAcceptedEventHandler;
            _connectionDeclined = connectionDeclinedEventHandler;

            try
            {
                BtnRejectConnection.Click += Btn_RejectConnection_Click;
            }
            catch (RejectConnectionException)
            {
                throw new RejectConnectionException();
            }
        }

        private void Btn_RejectConnection_Click(object sender, RoutedEventArgs e)
        {
            Close();
            _connectionDeclined.Invoke(this, new ConnectionDeclinedEventArgs());
        }

        private void BtnAcceptConnection_Click(object sender, RoutedEventArgs e)
        {
            Close();
            _connectionAccepted.Invoke(this, new ConnectionAcceptedEventArgs());
        }

        /*        public void ConnectionResponse(object sender, RoutedEventArgs e)
                {
                    FrameworkElement feSource = e.Source as FrameworkElement;
                    switch (feSource.Name)
                    {
                        case "BtnAcceptConnection":
                            break;

                        case "BtnRejectConnection":
                            userConnectionWindow.TerminateConnection();
                            break;

                        default:
                            break;
                    }
                    Close();
                    e.Handled = true;
                }*/
    }
}