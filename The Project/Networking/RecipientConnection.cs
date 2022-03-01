using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using The_Project.Accounts;
using The_Project.Exceptions;

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

        public async Task<bool> ConnectTo(UserId UserId)
        {
            int[] Ports = Enumerable.Range(UserId.MinPort, UserId.MaxPort).ToArray();

            Dispatcher? CurrentDispatcher = Dispatcher.CurrentDispatcher;

            Task<TcpClient?>[] Tasks = new Task<TcpClient?>[UserId.MaxPort - UserId.MinPort];
            for (int i = 0; i < (UserId.MaxPort - UserId.MinPort); i++)
            {
                try
                {
                    Tasks[i] = CreateConnection(UserId.IP, Ports[i], UserId.AccountId, CurrentDispatcher);
                }
                catch (ConnectionRefusedException e)
                {
                    continue;
                }
            }

            int index = Task.WaitAny(Tasks, 60000);

            Client = index > 0 ? await Tasks[index] : throw new ConnectionRefusedException($"Could not connect to {UserId.Id} on any port range!");
            return index > 0;
        }

        /*        public bool ConnectTo(UserId UserId)
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
                }*/

        public async Task<TcpClient?> CreateConnection(IPAddress IP, int Port, string AccountId, Dispatcher Dispatcher)
        {
            TcpClient Client = new();

            try
            {
                Dispatcher.Invoke(() => debugWindow?.Debug("Connecting to client..."));
                await Client.ConnectAsync(IP, Port);
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() => debugWindow?.Debug($"Could not connect to client on {IP}:{Port}!"));
                Dispatcher.Invoke(() => debugWindow?.Debug(e.Message));
                throw new ConnectionRefusedException($"Client refused connection: {e.Message}");
            }

            if (Client?.Connected ?? false)
            {
                Dispatcher.Invoke(() => debugWindow?.Debug("Connected to client!"));
                Dispatcher.Invoke(() => debugWindow?.Debug("Verifying account ID with connected client"));
                Client.GetStream().Write(Encoding.UTF8.GetBytes(AccountId));
                return Client;
            }
            Dispatcher.Invoke(() => debugWindow?.Debug($"Connection {((Client?.Connected ?? false) ? "success" : "failed")}"));

            throw new ConnectionRefusedException("Client refused connection");
        }
    }
}