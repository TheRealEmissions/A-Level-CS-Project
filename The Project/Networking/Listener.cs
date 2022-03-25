using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using The_Project.Accounts;
using The_Project.Extensions;

#nullable enable

namespace The_Project.Networking
{
    public class Listener
    {
        public TcpListener Server { get; }
        public int Port { get; }
        private readonly LoggingWindow? _loggingWindow;

        public Listener(UserId userId, LoggingWindow? loggingWindow = null)
        {
            _loggingWindow = loggingWindow;
            Debug.WriteLine(loggingWindow);
            loggingWindow?.Debug("Initialising Listener!");
            Port = GeneratePort(userId.MinPort, userId.MaxPort);
            Server = new TcpListener(Utils.GetLocalIpAddress(), Port);
            Server.Start();
        }

        private static int GeneratePort(int min, int max)
        {
            return new Random().Next(min, max);
        }

        public Task<RecipientConnection> ListenAndConnect(string accountId)
        {
            _loggingWindow?.Debug("Launching listening and connect process!");

            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
            string? accountIdBuffer;
            return Task.Run(() =>
            {
                // buffer
                byte[] bytesBuffer = new byte[256];
                RecipientConnection? recipientConnection = null;

                currentDispatcher.Invoke(() => _loggingWindow?.Debug("Waiting for connection..."));
                while (recipientConnection is null)
                {
                    // if no pending connection, continue loop
                    if (!Server.Pending())
                    {
                        continue;
                    }
                    currentDispatcher.Invoke(() => _loggingWindow?.Debug("Connection found! Connecting..."));
                    // accept pending connection
                    TcpClient tcpClient = Server.AcceptTcpClient();

                    // check account id
                    NetworkStream networkStream = tcpClient.GetStream();
                    while (networkStream.Read(bytesBuffer, 0, bytesBuffer.Length) != 0)
                    {
                        accountIdBuffer = Encoding.UTF8.GetString(bytesBuffer);
                        // verify account id
                        if (accountIdBuffer == accountId)
                        {
                            currentDispatcher.Invoke(() => _loggingWindow?.Debug("Verified account ID! Confirming connection..."));
                            networkStream.Write(new BitArray(new[] { true, true, true, true, true, true, true, true }).ToByteArray());
                            recipientConnection = new RecipientConnection(tcpClient);
                            currentDispatcher.Invoke(() => _loggingWindow?.Debug("Connection established!"));
                            break;
                        }
                        currentDispatcher.Invoke(() => _loggingWindow?.Debug("Account ID does not match! Terminating connection."));
                        networkStream.Write(new byte[1]);
                        tcpClient.Close();
                    }
                    Server.Stop();
                }
                return recipientConnection;
            });
        }
    }
}