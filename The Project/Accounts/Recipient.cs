using System.Diagnostics;

using System.Threading.Tasks;
using The_Project.Cryptography;
using The_Project.Networking;
using The_Project.Networking.Extensions;
using The_Project.Networking.Packets;

#nullable enable
namespace The_Project.Accounts
{
    internal sealed class Recipient
    {
        internal RecipientConnection Connection { get; set; }
        internal PublicKey PublicKey { get; set; }
        internal bool PublicKeyStored { get; set; }
        internal string? Nickname { get; set; }
        internal string? AccountId { get; set; }


        internal Recipient(RecipientConnection connection, PublicKey publicKey, string? accountId = null,
            string? nickname = null)
        {
            Connection = connection;
            PublicKey = publicKey;
            PublicKeyStored = true;
            Nickname = nickname;
            AccountId = accountId;
        }

        internal Recipient(RecipientConnection connection, string? accountId = null, string? nickname = null)
        {
            Connection = connection;
            PublicKeyStored = false;
            Nickname = nickname;
            AccountId = accountId;
        }

        /**
         * Send a message to the recipient
         */
        internal void Send(string text)
        {
            string cipherText = text.Encrypt(PublicKey);
            Debug.WriteLine("Sending message");
            Connection.TcpClient?.GetStream().WriteData(new Packet
                {Data = new MessagePacket {M = cipherText}, T = (int) PacketIdentifier.Packet.Message});
        }

        internal async Task SendPublicKey(PublicKey publicKey)
        {
            if (Connection.TcpClient is null)
            {
                return;
            }

            Debug.Write("Sending public key");
            await Connection.TcpClient.GetStream().WriteDataAsync(new Packet
            {
                Data = new PublicKeyPacket
                    {E = publicKey.E.ToString(), N = publicKey.N.ToString()},
                T = (int) PacketIdentifier.Packet.PublicKey
            });
        }

        internal async Task SendAccountId(string accountId)
        {
            if (Connection.TcpClient is null)
            {
                return;
            }

            Debug.WriteLine("Sending account ID");
            await Connection.TcpClient.GetStream().WriteDataAsync(new Packet
            {
                Data = new AccountIdPacket
                {
                    A = accountId
                },
                T = (int) PacketIdentifier.Packet.AccountId
            });
        }
    }
}