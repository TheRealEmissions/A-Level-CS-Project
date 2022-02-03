using System.Net;
using System.Windows;
using The_Project.Exceptions;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for ConnectionAccept.xaml
    /// </summary>
    public partial class ConnectionAcceptWindow : Window
    {
        private UserConnectionPage UserConnectionWindow { get; init; }

        public ConnectionAcceptWindow(UserConnectionPage UserConnectionWindow, IPAddress IP)
        {
            this.UserConnectionWindow = UserConnectionWindow;
            Txtblock_IPAddress.Text = IP.ToString();
            InitializeComponent();

            try
            {
                Btn_RejectConnection.Click += Btn_RejectConnection_Click;
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
                        case "Btn_AcceptConnection":
                            break;

                        case "Btn_RejectConnection":
                            UserConnectionWindow.TerminateConnection();
                            break;

                        default:
                            break;
                    }
                    Close();
                    e.Handled = true;
                }*/
    }
}