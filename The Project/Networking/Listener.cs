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
            // returns a task so it launches on a separate process and does not hold up the main thread
            return Task.Run<Task>(async () =>
            {
                while (recipient.Connection.TcpClient?.Connected ?? false)
                {
                    // buffer for storing incoming data
                    byte[] bytesBuffer = new byte[65536];
                    NetworkStream? networkStream = recipient.Connection.TcpClient?.GetStream();
                    // grab data from the stream when its available
                    while (networkStream?.DataAvailable ?? false)
                    {
                        // read the stream and store the data in the bytesBuffer as a memory location (more efficient)
                        int bytesRead = await networkStream.ReadAsync(bytesBuffer.AsMemory(0, bytesBuffer.Length));
                        if (bytesRead == 0)
                        {
                            continue;
                        }

                        // convert to a UTF-8 string
                        string utf8String = Encoding.UTF8.GetString(bytesBuffer);
                        bytesBuffer = new byte[65536];

                        // all packets end in $ to signify the end of the packet, hence splitting it by $ to get each packet that has been read
                        string[] splitByDelimiter = utf8String.Split("$");
                        // maps all packets to bytes from a utf-8 string
                        List<byte[]> packetsList =
                            splitByDelimiter.Select(static x => Encoding.UTF8.GetBytes(x)).ToList();
                        foreach (byte[] bytes in packetsList)
                        {
                            if (bytes.Length <= 0)
                            {
                                continue;
                            }


                            try
                            {
                                HandlePacket(bytes, recipient, userAccount, messagePage, dispatcher);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.Message);
                                Debug.WriteLine(e.StackTrace);
                            }


                        }
                    }
                }
            });
        }

        public Task<RecipientConnection?> ListenAndConnect(string accountId)
        {
            _loggingWindow?.Debug("Launching listening and connect process!");

            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
            AccountIdVerificationPacket? accountIdBuffer;
            // launches a task so it runs listening process on a separate thread and does not hold up the main thread
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

                    NetworkStream networkStream = tcpClient.GetStream();
                    // checking account id
                    // reads the data incoming (guaranteed to be the account ID packet) into the bytesBuffer
                    while (networkStream.CanRead && networkStream.Read(bytesBuffer, 0, bytesBuffer.Length) != 0)
                    {
                        // splits by $ as all packets end in $ to signify the end of the packet
                        string[] splitByDelimiter = Encoding.UTF8.GetString(bytesBuffer).Split("$");
                        // get the first packet (guaranteed to be the account ID packet)
                        byte[] packetBytes = Encoding.UTF8.GetBytes(splitByDelimiter[0]);

                        // deserializes the packet into a workable object (AccountIdVerificationPacket) AND
                        // gets rid of "junk" data (as the buffer is 256 in length, a packet may not fill all 256 bytes, leaving 0x00 behind, which can break deserialization)
                        accountIdBuffer =
                            JsonSerializer.Deserialize<AccountIdVerificationPacket>(packetBytes.ToList()
                                .Where(static x => x != 0).ToArray());
                        // verify account id
                        if (accountIdBuffer?.A == accountId)
                        {
                            currentDispatcher.Invoke(() =>
                                _loggingWindow?.Debug("Verified account ID! Confirming connection..."));
                            // returns a ConnectionVerifiedPacket with current user's account ID
                            networkStream.WriteData(new Packet
                            {
                                Data = new ConnectionVerifiedPacket {A = false, ID = accountId},
                                T = (int) PacketIdentifier.Packet.ConnectionVerified
                            });
                            recipientConnection = new RecipientConnection(tcpClient, _mainWindow, _loggingWindow);

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