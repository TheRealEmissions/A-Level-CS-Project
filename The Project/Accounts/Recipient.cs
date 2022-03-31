
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;
using The_Project.Cryptography;
using The_Project.Networking;
using The_Project.Networking.Packets;

#nullable enable
namespace The_Project.Accounts
{
    public class Recipient
    {
        public RecipientConnection Connection { get; }
        public PublicKey PublicKey { get; set; }
        public bool PublicKeyStored { get; set; }
        public string? Nickname { get; set; }

        public Recipient(RecipientConnection connection, PublicKey publicKey)
        {
            Connection = connection;
            PublicKey = publicKey;
        }

        public Recipient(RecipientConnection connection)
        {
            Connection = connection;
        }

        /**
         * Send a message to the recipient
         */

        public void Send(string text)
        {
            string cipherText = text.Encrypt(PublicKey);
            Debug.WriteLine("Sending message");
            Connection.TcpClient?.GetStream().Write(JsonSerializer.SerializeToUtf8Bytes(new Packet {Data = new MessagePacket { M = cipherText }, T = (int)PacketIdentifier.Packet.Message}));
        }

        public async Task SendPublicKey(PublicKey publicKey)
        {
            if (Connection.TcpClient is null)
            {
                return;
            }

            Debug.Write("Sending public key");
            await Connection.TcpClient.GetStream()
                .WriteAsync(JsonSerializer.SerializeToUtf8Bytes(new Packet
                {
                    Data = new PublicKeyPacket
                { E = publicKey.E, N = publicKey.N }, T = (int)PacketIdentifier.Packet.PublicKey}));
        }
    }
}