using System;

using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    [Serializable]
    public sealed class PublicKeyPacket : IPacket
    {
        public int T { get; set; } = 1;
        public string N { get; set; }
        public string E { get; set; }
    }
}
