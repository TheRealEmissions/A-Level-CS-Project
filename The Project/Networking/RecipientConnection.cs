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
        private LoggingWindow? DebugWindow { get; init; }

        public RecipientConnection(LoggingWindow? DebugWindow = null)
        {
            this.DebugWindow = DebugWindow;
        }

        public RecipientConnection(TcpClient Client, LoggingWindow? DebugWindow = null)
        {
            this.Client = Client;
            this.DebugWindow = DebugWindow;
        }

        public async Task<bool> ConnectTo(UserId UserId)
        {
            Dispatcher? CurrentDispatcher = Dispatcher.CurrentDispatcher;

            Task<TcpClient?>[] Tasks = new Task<TcpClient?>[UserId.MaxPort - UserId.MinPort];
            for (int port = UserId.MinPort; port < UserId.MinPort; port++)
            {
                try
                {
                    Tasks[port - UserId.MinPort] = CreateConnection(UserId.IP, port, UserId.AccountId, CurrentDispatcher);
                }
                catch (ConnectionRefusedException)
                {
                    continue;
                }
            }

            int index = Task.WaitAny(tasks: Tasks, 120000);

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
                Dispatcher.Invoke(() => DebugWindow?.Debug($"Connecting to client on {IP}:{Port}..."));
                await Client.ConnectAsync(IP, Port);
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() => DebugWindow?.Debug($"Could not connect to client on {IP}:{Port}!"));
                Dispatcher.Invoke(() => DebugWindow?.Debug(e.Message));
                throw new ConnectionRefusedException($"Client refused connection: {e.Message}");
            }

            if (Client?.Connected ?? false)
            {
                Dispatcher.Invoke(() => DebugWindow?.Debug("Connected to client!"));
                Dispatcher.Invoke(() => DebugWindow?.Debug("Verifying account ID with connected client"));
                Client.GetStream().Write(Encoding.UTF8.GetBytes(AccountId));
                return Client;
            }
            Dispatcher.Invoke(() => DebugWindow?.Debug($"Connection {Client switch { TcpClient client => $"success {client}", null => "failed" }}"));

            throw new ConnectionRefusedException("Client refused connection");
        }
    }
}