using System.Net;
using System.Net.Sockets;
using The_Project.Accounts;

#nullable enable

namespace The_Project.Networking
{
    public class RecipientConnection
    {
        public TcpClient? Client { get; set; }

        public RecipientConnection()
        {
        }

        public RecipientConnection(TcpClient Client)
        {
            this.Client = Client;
        }

        public bool ConnectTo(UserId UserId)
        {
            int PortToCheck = UserId.MinPort;
            while (Client is null && PortToCheck <= UserId.MaxPort)
            {
                Client = CreateConnection(UserId.IP, PortToCheck, UserId.AccountId);
                PortToCheck++;
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