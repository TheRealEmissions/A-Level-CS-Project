using System.Net;
using System.Windows;
using The_Project.Exceptions;

#nullable enable
namespace The_Project
{
    /// <summary>
    /// Interaction logic for ConnectionAccept.xaml
    /// </summary>
    public partial class ConnectionAcceptWindow
    {
        private readonly UserConnectionPage _userConnectionWindow;

        public bool ConnectionAccepted;

        public ConnectionAcceptWindow(UserConnectionPage userConnectionWindow, IPAddress? ipAddress)
        {
            if (ipAddress is null)
            {
                throw new CreateConnectionException("ip address not found for recipient");
            }

            InitializeComponent();

            _userConnectionWindow = userConnectionWindow;
            TxtblockIpAddress.Text = ipAddress.ToString();

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
            throw new RejectConnectionException();
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