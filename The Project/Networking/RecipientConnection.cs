using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Threading;
using The_Project.Accounts;
using The_Project.Exceptions;
using The_Project.Extensions;
using The_Project.Networking.Packets;

#nullable enable

namespace The_Project.Networking
{
    internal sealed class RecipientConnection
    {
        internal TcpClient? TcpClient { get; set; }
        private LoggingWindow? LoggingWindow { get; }

        internal bool ConnectionVerified { get; set; }
        internal bool ConnectionAccepted { get; set; }

        private MainWindow MainWindow { get; }

        internal RecipientConnection(MainWindow mainWindow, LoggingWindow? loggingWindow = null)
        {
            MainWindow = mainWindow;
            LoggingWindow = loggingWindow;
        }

        internal RecipientConnection(TcpClient tcpClient, MainWindow mainWindow, LoggingWindow? loggingWindow = null)
        {
            MainWindow = mainWindow;
            TcpClient = tcpClient;
            LoggingWindow = loggingWindow;
        }

        internal async Task<bool> ConnectTo(UserId userId)
        {
            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;

            List<Task<TcpClient?>> tasks = new();

            // launches a loop to search every port within the port range specified
            for (int port = userId.MinPort; port < userId.MaxPort; port++)
            {
                try
                {
                    LoggingWindow?.Debug($"Launching connection method for {userId.Ip}:{port}!");
                    // creates a task that launches on a separate process as to not hold up the main thread
                    Task<TcpClient?> taskConnection =
                        CreateConnection(userId.Ip, port, userId.AccountId, currentDispatcher);
                    tasks.Add(taskConnection);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            // uses First Success algorithm to find the first successfully connected TcpClient
            TcpClient = await tasks.FirstSuccessAsync();

            return TcpClient is not null;
        }

        private async Task<TcpClient?> CreateConnection(IPAddress ipAddress, int port, string accountId,
            Dispatcher dispatcher)
        {
            TcpClient tcpClient = new();

            try
            {
                dispatcher.Invoke(() => LoggingWindow?.Debug($"Connecting to client on {ipAddress}:{port}..."));

                // creates 2 tasks, one of which will time out (Task.Delay) if tcpClient.ConnectAsync does not return a connected client
                // required as timeout for ConnectAsync is too long and would reduce user satisfaction with the application
                Task timeoutTask = Task.Delay(20000);
                Task connectionTask = tcpClient.ConnectAsync(ipAddress, port);

                // assigns the task that completes first out of timeoutTask and connectionTask
                Task completedTask = await Task.WhenAny(timeoutTask, connectionTask);
                // if the completed task IS the timeout task
                if (completedTask == timeoutTask)
                {
                    throw new CreateConnectionException("No task completed in Create Connection!");
                }
            }
            catch (Exception e)
            {
                dispatcher.Invoke(() => LoggingWindow?.Debug($"Could not connect to client on {ipAddress}:{port}!"));
                dispatcher.Invoke(() => LoggingWindow?.Debug(e.Message));
                return null;
            }

            // previous checks ensured a timeout did not occur
            if (tcpClient.Connected)
            {
                dispatcher.Invoke(() => LoggingWindow?.Debug("Connected to client!"));
                dispatcher.Invoke(() => LoggingWindow?.Debug("Verifying account ID with connected client"));
                // issues AccountIdVerificationPacket with the recipient's account ID for verification
                await tcpClient.GetStream()
                    .WriteAsync(JsonSerializer.SerializeToUtf8Bytes(new AccountIdVerificationPacket {A = accountId}));
                return tcpClient;
            }


            return null;
        }
    }
}