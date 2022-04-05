using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using The_Project.Accounts;
using The_Project.Networking.Packets;
using The_Project.Cryptography;
using The_Project.Events;
using The_Project.Networking.Extensions;

namespace The_Project.Networking
{
    internal sealed partial class Listener
    {
        private static void HandlePacket(byte[] bytesBuffer, Recipient recipient, Account userAccount,
            MessagePage messagePage)
        {
            Debug.WriteLine("Bytes:");
            Debug.WriteLine(Encoding.UTF8.GetString(bytesBuffer));
            Packet packetBuffer = JsonSerializer.Deserialize<Packet>(bytesBuffer,
                new JsonSerializerOptions
                    {AllowTrailingCommas = true, IgnoreNullValues = true, DefaultBufferSize = bytesBuffer.Length});

            if (packetBuffer?.Data is null)
            {
                return;
            }

            switch ((PacketIdentifier.Packet) packetBuffer.T)
            {
                case PacketIdentifier.Packet.PublicKey:
                    HandlePublicKeyPacket(packetBuffer, recipient, userAccount);
                    break;
                case PacketIdentifier.Packet.Message:
                    HandleMessagePacket(packetBuffer, messagePage);
                    break;
                case PacketIdentifier.Packet.AccountIdVerification:
                    // no need to do anything here as connection handles this originally
                    break;
                case PacketIdentifier.Packet.ConnectionVerified:
                    HandleConnectionVerifiedPacket(packetBuffer, recipient, userAccount);
                    break;
                case PacketIdentifier.Packet.Exception:
                    // handle exception based on exception type
                    break;
            }
        }

        private static void HandlePublicKeyPacket(Packet packetBuffer, Recipient recipient, Account userAccount)
        {
            if (recipient.PublicKeyStored)
            {
                return;
            }

            PublicKeyPacket publicKeyPacket = JsonSerializer.Deserialize<PublicKeyPacket>(packetBuffer.Data.ToString());
            /*JsonSerializer.Deserialize<PublicKeyPacket>(packetBuffer.Data);*/
            Debug.WriteLine("\\/ Public Key \\/");
            Debug.WriteLine(publicKeyPacket);
            if (publicKeyPacket is null)
            {
                return;
            }

            Debug.WriteLine("Received Public Key");

            recipient.PublicKeyStored = true;
            recipient.PublicKey =
                new PublicKey(BigInteger.Parse(publicKeyPacket.N), BigInteger.Parse(publicKeyPacket.E));
            recipient.Connection.TcpClient?.GetStream().WriteData(new Packet
            {
                Data = new PublicKeyPacket
                    {E = userAccount.PublicKey.E.ToString(), N = userAccount.PublicKey.N.ToString()},
                T = (int) PacketIdentifier.Packet.PublicKey
            });
        }

        private static void HandleMessagePacket(Packet packetBuffer, MessagePage messagePage)
        {
            Debug.WriteLine("Received Message");
            MessagePacket messagePacket = JsonSerializer.Deserialize<MessagePacket>(packetBuffer.Data.ToString());
            /*JsonSerializer.Deserialize<MessagePacket>(((JsonElement) packetBuffer.Data)
                .GetString());*/
            Debug.WriteLine("\\/ Message Packet \\/");
            messagePage?.OnMessageReceived(new MessageReceivedEventArgs {Ciphertext = messagePacket?.M});
        }

        private static async void HandleConnectionVerifiedPacket(Packet packetBuffer, Recipient recipient,
            Account userAccount)
        {
            ConnectionVerifiedPacket connectionVerifiedPacket =
                JsonSerializer.Deserialize<ConnectionVerifiedPacket>(packetBuffer.Data.ToString());
            /*packetBuffer.Data as ConnectionVerifiedPacket*/
            ;
            /*JsonSerializer.Deserialize<ConnectionVerifiedPacket>(((JsonElement) packetBuffer.Data)
                .GetString());*/
            Debug.WriteLine("Connection Verified");
            if (recipient.Connection.ConnectionVerified && !recipient.Connection.ConnectionAccepted)
            {
                Debug.WriteLine("connection verified but not accepted");
                if (connectionVerifiedPacket?.A ?? false)
                {
                    Debug.WriteLine("Accepted connection");
                    recipient.Connection.ConnectionAccepted = true;

                    Debug.WriteLine("Sending public key");
                    await recipient.SendPublicKey(userAccount.PublicKey);
                }

                Debug.WriteLine("Connection is already verified");
                return;
            }

            if (recipient.Connection.ConnectionAccepted && recipient.Connection.ConnectionVerified)
            {
                Debug.WriteLine("Connection is accepted & verified");
                return;
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
            recipient.Connection.TcpClient?.GetStream().WriteDataAsync(new Packet
            {
                Data = new ConnectionVerifiedPacket {A = connectionAccepted},
                T = (int) PacketIdentifier.Packet.ConnectionVerified
            });
            if (!connectionVerifiedPacket?.A ?? true)
            {
                return;
            }

            Debug.WriteLine("Sending public key");
            await recipient.SendPublicKey(userAccount.PublicKey);
        }
    }
}