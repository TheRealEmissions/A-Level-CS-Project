
using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    public sealed class Packet : IPacket
    {
        public int T { get; set; }
        public object Data { get; set; }
    }
}
