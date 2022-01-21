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
        private readonly LoggingWindow? debugWindow;

        public Listener(UserId UserId, LoggingWindow? debugWindow = null)
        {
            this.debugWindow = debugWindow;
            Debug.WriteLine(debugWindow);
            debugWindow?.Debug("Initialising Listener!");
            Port = GeneratePort(UserId.MinPort, UserId.MaxPort);
            Server = new(IPAddress.Parse("127.0.0.1"), Port);
            Server.Start();
        }

        private static int GeneratePort(int Min, int Max)
        {
            return new Random().Next(Min, Max);
        }

        public Task<RecipientConnection> ListenAndConnect(string AccountId)
        {
            debugWindow?.Debug("Launching listening and connect process!");

            Dispatcher? CurrentDispatcher = Dispatcher.CurrentDispatcher;
            return Task.Run(() =>
            {
                // buffer
                byte[] bytesBuffer = new byte[256];
                string? accountIdBuffer;
                RecipientConnection? Connection = null;

                CurrentDispatcher.Invoke(() => debugWindow?.Debug("Waiting for connection..."));
                while (Connection is null)
                {
                    // if no pending connection, continue loop
                    if (!Server.Pending())
                    {
                        continue;
                    }
                    CurrentDispatcher.Invoke(() => debugWindow?.Debug("Connection found! Connecting..."));
                    // accept pending connection
                    TcpClient Client = Server.AcceptTcpClient();

                    // check account id
                    NetworkStream Stream = Client.GetStream();
                    while (Stream.Read(bytesBuffer, 0, bytesBuffer.Length) != 0)
                    {
                        accountIdBuffer = Encoding.UTF8.GetString(bytesBuffer);
                        // verify account id
                        if (accountIdBuffer == AccountId)
                        {
                            Stream.Write(new BitArray(new bool[8] { true, true, true, true, true, true, true, true }).ToByteArray());
                            Connection = new(Client);
                            break;
                        }
                        else
                        {
                            Stream.Write(new byte[1]);
                            Client.Close();
                        }
                    }
                    Server.Stop();
                }
                CurrentDispatcher.Invoke(() => debugWindow?.Debug("Connection established"));
                return Connection;
            });
        }
    }
}