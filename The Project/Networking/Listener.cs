using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;

using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Threading;
using The_Project.Accounts;

using The_Project.Networking.Extensions;
using The_Project.Networking.Packets;

#nullable enable

namespace The_Project.Networking
{
    internal sealed partial class Listener
    {
        internal TcpListener? Server { get; }
        internal int Port { get; }
        private readonly LoggingWindow? _loggingWindow;
        private readonly MainWindow _mainWindow;
        private readonly UserId _userId;

        internal Listener(UserId userId, MainWindow mainWindow, LoggingWindow? loggingWindow = null)
        {
            _loggingWindow = loggingWindow;
            _mainWindow = mainWindow;
            _userId = userId;
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

        public Task Poll(Account userAccount, Recipient recipient, MessagePage? messagePage = null)
        {
            Dispatcher dispatcher = Dispatcher.CurrentDispatcher;
            return Task.Run<Task>(async () =>
            {
                while (recipient.Connection.TcpClient?.Connected ?? false)
                {
                    byte[] bytesBuffer = new byte[65536];
                    NetworkStream? networkStream = recipient.Connection.TcpClient?.GetStream();
                    while (networkStream?.DataAvailable ?? false)
                    {
                        Debug.WriteLine(bytesBuffer.Length);
                        int bytesRead = await networkStream.ReadAsync(bytesBuffer.AsMemory(0, bytesBuffer.Length));
                        if (bytesRead == 0)
                        {
                            continue;
                        }

                        string utf8String = Encoding.UTF8.GetString(bytesBuffer);
                        bytesBuffer = new byte[65536];
                        Debug.WriteLine(utf8String);
                        string[] splitByDelimiter = utf8String.Split("$");
                        List<byte[]> packetsList =
                            splitByDelimiter.Select(static x => Encoding.UTF8.GetBytes(x)).ToList();
                        foreach (byte[] bytes in packetsList)
                        {
                            if (bytes.Length <= 0)
                            {
                                continue;
                            }

                            Debug.WriteLine(Encoding.UTF8.GetString(bytes));
                            try
                            {
                                HandlePacket(bytes, recipient, userAccount, messagePage, dispatcher);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                                Debug.WriteLine(e.StackTrace);
                            }

                            Debug.WriteLine("Handled packet!");
                        }

                        /*Debug.WriteLine(bytesBuffer.Length);
                        Debug.WriteLine(lastPosition);
                        Debug.WriteLine(bytesRead);
                        lastPosition += bytesRead;
                        Debug.WriteLine(lastPosition);
                        byte[] clonedBytes = bytesBuffer[(lastPosition - bytesRead)..(bytesRead - 1)].Clone() as byte[] ?? bytesBuffer;
                        bytesBuffer = new byte[16384];
                        lastPosition = 0;
                        HandlePacket(clonedBytes, recipient, userAccount, messagePage);
                        Debug.WriteLine("Handled packet!");*/
                    }
                }
            });
        }

        public Task<RecipientConnection?> ListenAndConnect(string accountId)
        {
            _loggingWindow?.Debug("Launching listening and connect process!");

            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
            AccountIdVerificationPacket? accountIdBuffer;
            return Task.Run(() =>
            {
                // buffer
                byte[] bytesBuffer = new byte[256];
                RecipientConnection? recipientConnection = null;

                currentDispatcher.Invoke(() => _loggingWindow?.Debug("Waiting for connection..."));
                while (recipientConnection is null && !Server.Server.Connected)
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
                    while (networkStream.CanRead && networkStream.Read(bytesBuffer, 0, bytesBuffer.Length) != 0)
                    {
                        string[] splitByDelimiter = Encoding.UTF8.GetString(bytesBuffer).Split("$");
                        byte[] packetBytes = Encoding.UTF8.GetBytes(splitByDelimiter[0]);
                        Debug.WriteLine("Bytes (second):");
                        Debug.WriteLine(Encoding.UTF8.GetString(packetBytes));
                        accountIdBuffer =
                            JsonSerializer.Deserialize<AccountIdVerificationPacket>(packetBytes.ToList()
                                .Where(static x => x != 0).ToArray());
                        // verify account id
                        if (accountIdBuffer?.A == accountId)
                        {
                            currentDispatcher.Invoke(() =>
                                _loggingWindow?.Debug("Verified account ID! Confirming connection..."));
                            networkStream.WriteData(new Packet
                            {
                                Data = new ConnectionVerifiedPacket {A = false, ID = accountId},
                                T = (int) PacketIdentifier.Packet.ConnectionVerified
                            });
                            recipientConnection = new RecipientConnection(tcpClient, _mainWindow, _loggingWindow);
                            /*Database.RecipientAccount recipientAccountDatabase = new(_mainWindow.Handler.Connection,
                                _mainWindow.Handler.UserAccount, _mainWindow.Handler.Tables);
                            if (recipientAccountDatabase.HasAccount(accountIdBuffer?.A))
                            {
                                Debug.WriteLine("Got recipient account!");
                                Database.Tables.RecipientAccount.RecipientAccountSchema? schema = recipientAccountDatabase.GetAccount(accountIdBuffer?.A);
                                Debug.WriteLine($"{schema}");
                                _mainWindow.Handler.Recipient = new Recipient(recipientConnection, schema?.AccountId);
                            }
                            else
                            {
                                Debug.WriteLine("Creating new recipient account");
                                Debug.WriteLine($"{_mainWindow.Handler.UserAccount?.AccountId}");
                                recipientAccountDatabase.CreateAccount(accountIdBuffer?.A);
                                _mainWindow.Handler.Recipient = new Recipient(recipientConnection, accountIdBuffer?.A);
                            }*/

                            currentDispatcher.Invoke(() => _loggingWindow?.Debug("Connection established!"));
                            Server.Stop();
                            break;
                        }

                        currentDispatcher.Invoke(() =>
                            _loggingWindow?.Debug("Account ID does not match! Terminating connection."));
                        networkStream.WriteData(new Packet
                        {
                            Data = new ExceptionPacket {E = 0, S = "FAILED_ACCOUNT_ID"},
                            T = (int) PacketIdentifier.Packet.Exception
                        });
                        tcpClient.Close();
                    }

                    break;
                }

                return recipientConnection;
            });
        }
    }
}