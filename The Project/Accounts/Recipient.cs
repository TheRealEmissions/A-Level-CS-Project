using System.Diagnostics;
using System.Text.Json;
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
        internal RecipientConnection Connection { get; }
        internal PublicKey PublicKey { get; set; }
        internal bool PublicKeyStored { get; set; }
        internal string? Nickname { get; set; }
        internal string AccountId { get; set; }

        public Recipient(RecipientConnection connection, PublicKey publicKey)
        {
            Connection = connection;
            PublicKey = publicKey;
        }

        internal Recipient(RecipientConnection connection)
        {
            Connection = connection;
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
    }
}