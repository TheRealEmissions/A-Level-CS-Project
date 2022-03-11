using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

            List<Task<TcpClient?>> Tasks = new();

            for (int port = UserId.MinPort; port < UserId.MaxPort; port++)
            {
                try
                {
                    DebugWindow?.Debug($"Launching connection method for {UserId.IP}:{port}!");
                    Task<TcpClient?> TaskConnection = CreateConnection(UserId.IP, port, UserId.AccountId, CurrentDispatcher);
                    Tasks.Add(TaskConnection);
                }
                catch (ConnectionRefusedException)
                {
                    Debug.WriteLine("Error occurred when creating connection");
                    continue;
                }
            }

            /*            while (Tasks.Count > 0)
                        {
                            Task<TcpClient?> resultTask = await Task.WhenAny(Tasks);
                            TcpClient? result = await resultTask;
                            if (result is null)
                            {
                                Tasks.Remove(resultTask);
                            }
                            else
                            {
                                CancellationToken.Cancel();
                                Client = result is TcpClient ? result : throw new ConnectionRefusedException("Returned task is not a TcpClient!");
                                break;
                            }
                        }*/
            Debug.WriteLine(Tasks.Count);
            try { Client = await Extensions.TaskExtension<TcpClient>.FirstSuccess(Tasks); }
            catch (Exception)
            {
                throw;
            }

            return Client is TcpClient;
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
            TcpClient? Client = new();

            try
            {
                Dispatcher.Invoke(() => DebugWindow?.Debug($"Connecting to client on {IP}:{Port}..."));
                Debug.WriteLine($"Connecting to client on {IP}:{Port}...");
                /*await Client.ConnectAsync(IP, Port);*/
                Task? timeoutTask = Task.Delay(5000);
                Task? connectionTask = Client.ConnectAsync(IP, Port);

                Task? completedTask = await Task.WhenAny(timeoutTask, connectionTask);
                if (completedTask is null || completedTask == timeoutTask)
                {
                    Debug.WriteLine(completedTask is null ? "completedTask is null" : $"timeout reached for {IP}:{Port}");
                    throw new Exception();
                }
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() => DebugWindow?.Debug($"Could not connect to client on {IP}:{Port}!"));
                Debug.WriteLine($"Could not connect to client on {IP}:{Port}!");
                Dispatcher.Invoke(() => DebugWindow?.Debug(e.Message));
                Client = null;
                /*throw new ConnectionRefusedException($"Client refused connection: {e.Message}");*/
                return null;
            }

            if (Client?.Connected ?? false)
            {
                Dispatcher.Invoke(() => DebugWindow?.Debug("Connected to client!"));
                Debug.WriteLine("Connected to client!");
                Dispatcher.Invoke(() => DebugWindow?.Debug("Verifying account ID with connected client"));
                Client.GetStream().Write(Encoding.UTF8.GetBytes(AccountId));
                return Client;
            }
            Dispatcher.Invoke(() => DebugWindow?.Debug($"Connection {Client switch { TcpClient client => $"success {client}", null => "failed" }}"));
            Debug.WriteLine($"Connection {Client switch { TcpClient client => $"success {client}", null => "failed" }}");

            /*throw new ConnectionRefusedException("Client refused connection");*/
            return null;
        }
    }
}