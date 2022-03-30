using System.Numerics;
using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    public class PublicKeyPacket : IPacket
    {
        public int T { get; set; } = 1;
        public BigInteger N { get; set; }
        public BigInteger E { get; set; }
    }
}
