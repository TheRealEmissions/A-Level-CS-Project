using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using The_Project.Accounts;

#nullable enable
namespace The_Project.Networking
{
    public class RecipientConnection
    {

        public TcpClient? Client { get; }

        public RecipientConnection()
        {
            
        }

        public RecipientConnection(TcpClient Client)
        {
            this.Client = Client;
        }

        public static bool ConnectTo(UserId UserId)
        {
            TcpClient? Client = null;
            int PortToCheck = UserId.MinPort;
            while (Client is null)
            {
                Client = CreateConnection(UserId.IP, PortToCheck, UserId.AccountId);
            }
            return Client is null;
        }

        public static TcpClient? CreateConnection(IPAddress IP, int Port, string AccountId)
        {
            TcpClient Client = new(IP.ToString(), Port);
            return Client.Connected ? Client : null;
        }
    }
}
