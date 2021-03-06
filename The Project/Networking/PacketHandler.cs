
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Threading;
using The_Project.Accounts;
using The_Project.Networking.Packets;
using The_Project.Cryptography;
using The_Project.Events;
using The_Project.Networking.Extensions;

namespace The_Project.Networking
{
    internal sealed partial class Listener
    {
        private void HandlePacket(IReadOnlyCollection<byte> bytesBuffer, Recipient recipient,
            Account userAccount,
            MessagePage messagePage, Dispatcher mainThreadDispatcher)
        {
            // gets rid of "junk" data in the buffer, as buffer can be bigger than packet size
            // if packet size is 20 bytes and buffer is 100 bytes, 80 bytes will be 0x00 which breaks deserialization
            byte[] filteredBytes = bytesBuffer.ToList().Where(static x => x != 0).ToArray();
            if (filteredBytes.Length <= 0)
            {
                return;
            }

            Packet packetBuffer = JsonSerializer.Deserialize<Packet>(filteredBytes,
                new JsonSerializerOptions
                    {AllowTrailingCommas = true, IgnoreNullValues = true, DefaultBufferSize = filteredBytes.Length});

            if (packetBuffer?.Data is null)
            {
                return;
            }

            // casts T (the type of the packet) to the enum specific counterpart
            switch ((PacketIdentifier.Packet) packetBuffer.T)
            {
                case PacketIdentifier.Packet.PublicKey:
                    HandlePublicKeyPacket(packetBuffer, recipient, userAccount);
                    break;
                case PacketIdentifier.Packet.Message:
                    HandleMessagePacket(packetBuffer, messagePage, mainThreadDispatcher);
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
                case PacketIdentifier.Packet.AccountId:
                    HandleAccountIdPacket(packetBuffer, recipient);
                    break;
            }
        }

        private async void HandleAccountIdPacket(Packet packetBuffer, Recipient recipient)
        {
            if (recipient.AccountIdSent)
            {
                return;
            }

            AccountIdPacket accountPacket =
                JsonSerializer.Deserialize<AccountIdPacket>(packetBuffer.Data.ToString() ?? string.Empty);

            Database.RecipientAccount recipientAccount = new(
                _mainWindow.Handler.UserAccount, _mainWindow.Handler.Tables);
            recipient.AccountId = accountPacket?.A;
            if (!recipientAccount.HasAccount(accountPacket?.A))
            {
                recipientAccount.CreateAccount(accountPacket?.A);
            }

            recipient.AccountIdStored = true;

            if (!recipient.AccountIdSent)
            {
                recipient.AccountIdSent = true;
                await recipient.SendAccountId(_mainWindow.Handler.UserAccount.AccountId);
            }
        }

        private void HandlePublicKeyPacket(Packet packetBuffer, Recipient recipient, Account userAccount)
        {
            if (recipient.PublicKeyStored)
            {
                return;
            }

            PublicKeyPacket publicKeyPacket = JsonSerializer.Deserialize<PublicKeyPacket>(packetBuffer.Data.ToString() ?? string.Empty);

            if (publicKeyPacket is null)
            {
                return;
            }

            // successfully received recipient's public key at this point

            recipient.PublicKeyStored = true;
            recipient.PublicKey =
                new PublicKey(BigInteger.Parse(publicKeyPacket.N), BigInteger.Parse(publicKeyPacket.E));

            // sends back current user's public key to be used by the recipient
            recipient.Connection.TcpClient?.GetStream().WriteData(new Packet
            {
                Data = new PublicKeyPacket
                    {E = userAccount.PublicKey.E.ToString(), N = userAccount.PublicKey.N.ToString()},
                T = (int) PacketIdentifier.Packet.PublicKey
            });
        }

        private void HandleMessagePacket(Packet packetBuffer, MessagePage messagePage, Dispatcher dispatcher)
        {
            // received a message, deserializes the stream data to a MessagePacket
            MessagePacket messagePacket = JsonSerializer.Deserialize<MessagePacket>(packetBuffer.Data.ToString() ?? string.Empty);

            // sends an event to the main thread which handles message receiving
            dispatcher.Invoke(() =>
                messagePage?.OnMessageReceived(new MessageReceivedEventArgs {Ciphertext = messagePacket?.M}));
        }

        private async Task HandleConnectionVerifiedPacket(Packet packetBuffer, Recipient recipient,
            Account userAccount)
        {
            ConnectionVerifiedPacket connectionVerifiedPacket =
                JsonSerializer.Deserialize<ConnectionVerifiedPacket>(packetBuffer.Data.ToString() ?? string.Empty);
            recipient.AccountId = connectionVerifiedPacket?.ID;
            
            // if the connection is verified (account id that the recipient sent has been checked to be correct) BUT the connection is not yet accepted
            if (recipient.Connection.ConnectionVerified && !recipient.Connection.ConnectionAccepted)
            {
                // if the packet has A = true, which means the recipient has accepted the connection
                if (connectionVerifiedPacket?.A ?? false)
                {

                    recipient.Connection.ConnectionAccepted = true;
                    recipient.AccountId = connectionVerifiedPacket.ID;

                    // sends public key to the recipient
                    await recipient.SendPublicKey(userAccount.PublicKey);
                    // sends user's account id to the recipient
                    await recipient.SendAccountId(_mainWindow.Handler.UserAccount.AccountId);
                }

                // otherwise ignore
                return;
            }

            // if connection is accepted & verified, no need to do anything, ignore packet
            if (recipient.Connection.ConnectionAccepted && recipient.Connection.ConnectionVerified)
            {
                return;
            }

            
            recipient.Connection.ConnectionVerified = true;
            // if the connection has been accepted by the recipient
            if (connectionVerifiedPacket?.A ?? false)
            {
                recipient.Connection.ConnectionAccepted = true;
                recipient.AccountId = connectionVerifiedPacket.ID;
            }

            bool connectionAccepted = connectionVerifiedPacket?.A ?? false;
            // returning connection verified packet providing TcpClient is not null (still connected to the recipient)
            if (recipient.Connection.TcpClient is not null)
            {
                await recipient.Connection.TcpClient.GetStream().WriteDataAsync(new Packet
                {
                    Data = new ConnectionVerifiedPacket {A = connectionAccepted, ID = userAccount.AccountId},
                    T = (int) PacketIdentifier.Packet.ConnectionVerified
                });
            }

            // if the connection hasn't been accepted, do nothing
            if (!connectionVerifiedPacket?.A ?? true)
            {
                return;
            }

            // otherwise send user's public key to the recipient
            await recipient.SendPublicKey(userAccount.PublicKey);
        }
    }
}