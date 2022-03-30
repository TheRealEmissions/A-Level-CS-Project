using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    public class ConnectionVerifiedPacket : IPacket
    {
        public int T { get; set; } = 4;
    }
}
