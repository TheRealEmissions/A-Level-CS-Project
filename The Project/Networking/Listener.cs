﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Threading;
using The_Project.Accounts;
using The_Project.Cryptography;
using The_Project.Events;
using The_Project.Networking.Packets;

#nullable enable

namespace The_Project.Networking
{
    internal sealed class Listener
    {
        private TcpListener Server { get; }
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

        public static async Task Poll(Account userAccount, Recipient recipient, MessagePage? messagePage = null)
        {

            while (recipient.Connection.TcpClient?.Connected ?? false)
            {
                byte[] bytesBuffer = new byte[1024];
                NetworkStream? networkStream = recipient.Connection.TcpClient?.GetStream();
                while ((networkStream?.CanRead ?? false) && await networkStream.ReadAsync(bytesBuffer.AsMemory(0, bytesBuffer.Length)) != 0)
                {
                    Packet? packetBuffer = JsonSerializer.Deserialize<Packet>(bytesBuffer.ToList().Where(static x => x != 0).ToArray());
                    Debug.WriteLine(packetBuffer?.Data);
                    if (packetBuffer is null)
                    {
                        continue;
                    }
                    switch ((PacketIdentifier.Packet)packetBuffer.T)
                    {
                        case PacketIdentifier.Packet.PublicKey:
                            if (recipient.PublicKeyStored)
                            {
                                return;
                            }

                            PublicKeyPacket? publicKeyPacket = packetBuffer.Data as PublicKeyPacket;
                                /*JsonSerializer.Deserialize<PublicKeyPacket>(packetBuffer.Data);*/
                            Debug.WriteLine("\\/ Public Key \\/");
                            Debug.WriteLine(publicKeyPacket);
                            if (publicKeyPacket is null)
                            {
                                break;
                            }
                            Debug.WriteLine("Received Public Key");

                            recipient.PublicKeyStored = true;
                            recipient.PublicKey = new PublicKey(publicKeyPacket.N, publicKeyPacket.E);
                            recipient.Connection.TcpClient?.GetStream().Write(JsonSerializer.SerializeToUtf8Bytes(new Packet {Data = new PublicKeyPacket { E = userAccount.PublicKey.E, N = userAccount.PublicKey.N }, T = (int)PacketIdentifier.Packet.PublicKey}));
                            break;
                        case PacketIdentifier.Packet.Message:
                            Debug.WriteLine("Received Message");
                            MessagePacket? messagePacket = packetBuffer.Data as MessagePacket;
                                /*JsonSerializer.Deserialize<MessagePacket>(((JsonElement) packetBuffer.Data)
                                    .GetString());*/
                            Debug.WriteLine("\\/ Message Packet \\/");
                            messagePage?.OnMessageReceived(new MessageReceivedEventArgs { Ciphertext = messagePacket?.M });
                            break;
                        case PacketIdentifier.Packet.AccountIdVerification:
                            // no need to do anything here as connection handles this originally
                            break;
                        case PacketIdentifier.Packet.ConnectionVerified:
                            ConnectionVerifiedPacket? connectionVerifiedPacket =
                                packetBuffer.Data as ConnectionVerifiedPacket;
                                /*JsonSerializer.Deserialize<ConnectionVerifiedPacket>(((JsonElement) packetBuffer.Data)
                                    .GetString());*/
                            Debug.WriteLine("Connection Verified");
                            if (recipient.Connection.ConnectionVerified && !recipient.Connection.ConnectionAccepted)
                            {
                                Debug.WriteLine("Connection is already verified (accepted is false)");
                                break;
                            }

                            if (recipient.Connection.ConnectionAccepted && recipient.Connection.ConnectionVerified)
                            {
                                Debug.WriteLine("Connection is accepted & verified");
                                break;
                            }
                            Debug.WriteLine("verified connection");
                            recipient.Connection.ConnectionVerified = true;
                            if (connectionVerifiedPacket?.A ?? false)
                            {
                                Debug.WriteLine("Connection accepted");
                                recipient.Connection.ConnectionAccepted = true;
                            }

                            bool connectionAccepted = connectionVerifiedPacket?.A ?? false;
                            Debug.WriteLine("Returning connection verified packet");
                            recipient.Connection.TcpClient?.GetStream().Write(JsonSerializer.SerializeToUtf8Bytes(new Packet { Data = new ConnectionVerifiedPacket { A = connectionAccepted }, T = (int)PacketIdentifier.Packet.ConnectionVerified }));
                            if (connectionVerifiedPacket?.A ?? false)
                            {
                                Debug.WriteLine("Sending public key");
                                await recipient.SendPublicKey(userAccount.PublicKey);
                            }
                            break;
                        case PacketIdentifier.Packet.Exception:
                            // handle exception based on exception type
                            break;
                    }
                }
            }
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
                        accountIdBuffer = JsonSerializer.Deserialize<AccountIdVerificationPacket>(bytesBuffer.ToList().Where(static x => x != 0).ToArray());
                        // verify account id
                        if (accountIdBuffer?.A == accountId)
                        {
                            currentDispatcher.Invoke(() => _loggingWindow?.Debug("Verified account ID! Confirming connection..."));
                            networkStream.Write(JsonSerializer.SerializeToUtf8Bytes(new Packet { Data = new ConnectionVerifiedPacket(), T = (int)PacketIdentifier.Packet.ConnectionVerified}));
                            recipientConnection = new RecipientConnection(tcpClient, _mainWindow, _loggingWindow);
                            currentDispatcher.Invoke(() => _loggingWindow?.Debug("Connection established!"));
                            Server.Stop();
                            break;
                        }
                        currentDispatcher.Invoke(() => _loggingWindow?.Debug("Account ID does not match! Terminating connection."));
                        networkStream.Write(JsonSerializer.SerializeToUtf8Bytes(new Packet { Data = new ExceptionPacket { E = 0, S = "FAILED_ACCOUNT_ID" }, T = (int)PacketIdentifier.Packet.Exception}));
                        tcpClient.Close();
                    }
                    break;
                }
                return recipientConnection;
            });
        }
    }
}