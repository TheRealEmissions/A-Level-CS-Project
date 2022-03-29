using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using The_Project.Accounts;

namespace The_Project
{
    /// <summary>
    /// Interaction logic for MessagePage.xaml
    /// </summary>
    public partial class MessagePage : Page
    {
        private UserId SelfUserId;
        private IPAddress RecipientIpAddress;
        public MessagePage(UserId selfUserId, IPAddress recipientIpAddress)
        {
            SelfUserId = selfUserId;
            RecipientIpAddress = recipientIpAddress;

            InitializeComponent();

            TxtblockUser.Text = recipientIpAddress.ToString();
        }
    }
}
