using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Threading;
using The_Project.Accounts;
using The_Project.Cryptography;
using The_Project.Events;
using The_Project.Networking.Packets;
using The_Project.Networking.Packets.Interfaces;

#nullable enable

namespace The_Project.Networking
{
    public class Listener
    {
        public TcpListener Server { get; }
        public int Port { get; }
        private readonly LoggingWindow? _loggingWindow;
        private readonly UserId _userId;

        public Listener(UserId userId, LoggingWindow? loggingWindow = null)
        {
            _loggingWindow = loggingWindow;
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

        public async Task Poll(Account userAccount, Recipient recipient, MessagePage? messagePage = null)
        {
            _loggingWindow?.Debug("Polling TCP connection for incoming packets!");

            while (recipient.Connection.TcpClient?.Connected ?? false)
            {
                byte[] bytesBuffer = new byte[1024];
                NetworkStream? networkStream = recipient.Connection.TcpClient?.GetStream();
                while ((networkStream?.CanRead ?? false) && await networkStream.ReadAsync(bytesBuffer.AsMemory(0, bytesBuffer.Length)) != 0)
                {
                    IPacket? packetBuffer = JsonSerializer.Deserialize<IPacket>(bytesBuffer);
                    if (packetBuffer is null)
                    {
                        continue;
                    }
                    switch ((PacketIdentifier.Packet)packetBuffer.T)
                    {
                        case PacketIdentifier.Packet.PublicKey:
                            recipient.PublicKey = packetBuffer is not PublicKeyPacket publicKeyPacket ? new PublicKey() : new PublicKey(publicKeyPacket.N, publicKeyPacket.E);
                            break;
                        case PacketIdentifier.Packet.Message:
                            MessagePacket? messagePacket = packetBuffer as MessagePacket;
                            messagePage?.OnMessageReceived(new MessageReceivedEventArgs { Ciphertext = messagePacket?.M });
                            break;
                        case PacketIdentifier.Packet.AccountIdVerification:
                            // no need to do anything here as connection handles this originally
                            break;
                        case PacketIdentifier.Packet.ConnectionVerified:
                            if (recipient.Connection.ConnectionVerified)
                            {
                                break;
                            }
                            recipient.Connection.ConnectionVerified = true;
                            await recipient.SendPublicKey(userAccount.PublicKey);
                            break;
                        case PacketIdentifier.Packet.Exception:
                            // handle exception based on exception type
                            break;
                    }
                }
            }
        }

        public Task<RecipientConnection> ListenAndConnect(string accountId)
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
                    while (networkStream.CanRead && networkStream.Read(bytesBuffer, 0, bytesBuffer.Length) != 0)
                    {
                        accountIdBuffer = JsonSerializer.Deserialize<AccountIdVerificationPacket>(bytesBuffer);
                        // verify account id
                        if (accountIdBuffer?.A[..2] == accountId)
                        {
                            currentDispatcher.Invoke(() => _loggingWindow?.Debug("Verified account ID! Confirming connection..."));
                            networkStream.Write(JsonSerializer.SerializeToUtf8Bytes(new ConnectionVerifiedPacket()));
                            recipientConnection = new RecipientConnection(tcpClient, _loggingWindow);
                            currentDispatcher.Invoke(() => _loggingWindow?.Debug("Connection established!"));
                            break;
                        }
                        currentDispatcher.Invoke(() => _loggingWindow?.Debug("Account ID does not match! Terminating connection."));
                        networkStream.Write(JsonSerializer.SerializeToUtf8Bytes(new ExceptionPacket { E = 0, S = "FAILED_ACCOUNT_ID" }));
                        tcpClient.Close();
                    }
                    Server.Stop();
                }
                return recipientConnection;
            });
        }
    }
}