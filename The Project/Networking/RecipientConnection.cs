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
        private LoggingWindow? debugWindow { get; init; }

        public RecipientConnection(LoggingWindow? debugWindow = null)
        {
            this.debugWindow = debugWindow;
        }

        public RecipientConnection(TcpClient Client, LoggingWindow? debugWindow = null)
        {
            this.Client = Client;
            this.debugWindow = debugWindow;
        }

        public bool ConnectTo(UserId UserId)
        {
            int PortToCheck = UserId.MinPort;
            debugWindow?.Debug($"Connecting to IP {UserId.IP}");
            while (Client is null && PortToCheck <= UserId.MaxPort)
            {
                debugWindow?.Debug($"Trying connection on port {PortToCheck}");
                Client = CreateConnection(UserId.IP, PortToCheck, UserId.AccountId);
                PortToCheck++;
            }
            return Client is null;
        }

        public TcpClient? CreateConnection(IPAddress IP, int Port, string AccountId)
        {
            TcpClient? Client = this.Client;
            try
            {
                Client = new(IP.ToString(), Port);
            }
            catch (System.Exception e)
            {
                debugWindow?.Debug($"Could not connect to {IP}:{Port}");
                debugWindow?.Debug(e.Message);
                return null;
            }

            debugWindow?.Debug("Connecting to client...");
            if (Client?.Connected ?? false)
            {
                debugWindow?.Debug("Connected to client!");
                debugWindow?.Debug("Verifying account ID with connected client");
                Client.GetStream().Write(Encoding.UTF8.GetBytes(AccountId));
            }
            debugWindow?.Debug($"Connection {((Client?.Connected ?? false) ? "success" : "failed")}");
            return (Client?.Connected ?? false) ? Client : null;
        }
    }
}