
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
        public string? Nickname { get; set; }

        public Recipient(RecipientConnection connection, PublicKey publicKey)
        {
            Connection = connection;
            PublicKey = publicKey;
        }

        /**
         * Send a message to the recipient
         */

        public void Send(string text)
        {
            string cipherText = text.Encrypt(PublicKey);
            Connection.TcpClient?.GetStream().Write(JsonSerializer.SerializeToUtf8Bytes(new MessagePacket { M = cipherText }));
        }

        public async Task SendPublicKey(PublicKey publicKey)
        {
            if (Connection.TcpClient is null)
            {
                return;
            }

            await Connection.TcpClient.GetStream()
                .WriteAsync(JsonSerializer.SerializeToUtf8Bytes(new PublicKeyPacket
                { E = publicKey.E, N = publicKey.N }));
        }
    }
}