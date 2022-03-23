using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using The_Project.Accounts;
using The_Project.Exceptions;
using The_Project.Extensions;

#nullable enable

namespace The_Project.Networking
{
    public class RecipientConnection
    {
        public TcpClient? TcpClient { get; set; }
        private LoggingWindow? LoggingWindow { get; }

        public RecipientConnection(LoggingWindow? loggingWindow = null)
        {
            this.LoggingWindow = loggingWindow;
        }

        public RecipientConnection(TcpClient tcpClient, LoggingWindow? loggingWindow = null)
        {
            this.TcpClient = tcpClient;
            this.LoggingWindow = loggingWindow;
        }

        public async Task<bool> ConnectTo(UserId userId)
        {
            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;

            List<Task<TcpClient?>> tasks = new();

            for (int port = userId.MinPort; port < userId.MaxPort; port++)
            {
                try
                {
                    LoggingWindow?.Debug($"Launching connection method for {userId.Ip}:{port}!");
                    Task<TcpClient?> taskConnection =
                        CreateConnection(userId.Ip, port, userId.AccountId, currentDispatcher);
                    tasks.Add(taskConnection);
                }
                catch (ConnectionRefusedException)
                {
                    Debug.WriteLine("Error occurred when creating connection");
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
                                tcpClient = result is TcpClient ? result : throw new ConnectionRefusedException("Returned task is not a TcpClient!");
                                break;
                            }
                        }*/
            Debug.WriteLine(tasks.Count);
            TcpClient = await tasks.FirstSuccessAsync();

            return TcpClient is not null;
        }

        /*        public bool ConnectTo(userId userId)
                {
                    int PortToCheck = userId.MinPort;
                    debugWindow?.Debug($"Connecting to Ip {userId.Ip}");
                    while (tcpClient is null && PortToCheck <= userId.MaxPort)
                    {
                        debugWindow?.Debug($"Trying connection on port {PortToCheck}");
                        tcpClient = CreateConnection(userId.Ip, PortToCheck, userId.accountId);
                        PortToCheck++;
                    }
                    return tcpClient is null;
                }*/

        public async Task<TcpClient?> CreateConnection(IPAddress ipAddress, int port, string accountId, Dispatcher dispatcher)
        {
            TcpClient? tcpClient = new();

            try
            {
                dispatcher.Invoke(() => LoggingWindow?.Debug($"Connecting to client on {ipAddress}:{port}..."));
                Debug.WriteLine($"Connecting to client on {ipAddress}:{port}...");
                /*await tcpClient.ConnectAsync(Ip, port);*/
                Task timeoutTask = Task.Delay(5000);
                Task connectionTask = tcpClient.ConnectAsync(ipAddress, port);

                Task? completedTask = await Task.WhenAny(timeoutTask, connectionTask);
                if (completedTask is null || completedTask == timeoutTask)
                {
                    Debug.WriteLine(
                        completedTask is null ? "completedTask is null" : $"timeout reached for {ipAddress}:{port}");
                    throw new CreateConnectionException("No task completed in Create Connection!");
                }
            }
            catch (Exception e)
            {
                dispatcher.Invoke(() => LoggingWindow?.Debug($"Could not connect to client on {ipAddress}:{port}!"));
                Debug.WriteLine($"Could not connect to client on {ipAddress}:{port}!");
                dispatcher.Invoke(() => LoggingWindow?.Debug(e.Message));
                /*throw new ConnectionRefusedException($"tcpClient refused connection: {e.Message}");*/
                return null;
            }

            if (tcpClient.Connected)
            {
                dispatcher.Invoke(() => LoggingWindow?.Debug("Connected to client!"));
                Debug.WriteLine("Connected to client!");
                dispatcher.Invoke(() => LoggingWindow?.Debug("Verifying account ID with connected client"));
                tcpClient.GetStream().Write(Encoding.UTF8.GetBytes(accountId));
                return tcpClient;
            }

            dispatcher.Invoke(() =>
                LoggingWindow?.Debug(
                    $"Connection {tcpClient switch { TcpClient client => $"success {client}", null => "failed" }}"));
            Debug.WriteLine($"Connection {tcpClient switch { TcpClient client => $"success {client}", null => "failed" }}");

            /*throw new ConnectionRefusedException("tcpClient refused connection");*/
            return null;
        }
    }
}