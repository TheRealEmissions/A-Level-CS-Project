using System.Net;
using System.Net.Sockets;
using System.Text;
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

        public TcpClient? CreateConnection(IPAddress IP, int Port, string AccountId)
        {
            TcpClient Client = this.Client ?? new(IP.ToString(), Port);
            if (Client.Connected)
            {
                Client.GetStream().Write(Encoding.UTF8.GetBytes(AccountId));
            }
            return Client.Connected ? Client : null;
        }
    }
}