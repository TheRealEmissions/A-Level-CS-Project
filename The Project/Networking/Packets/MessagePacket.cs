using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    public class MessagePacket : IPacket
    {
        // packet type (2)
        public int T { get; set; } = 2;
        // message content (ciphertext)
        public string M { get; set; }
    }
}
