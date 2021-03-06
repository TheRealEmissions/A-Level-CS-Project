using System;
using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    [Serializable]
    public sealed class ConnectionVerifiedPacket : IPacket
    {
        public int T { get; set; } = 4;
        public bool A { get; set; }
        public string ID { get; set; }
    }
}
