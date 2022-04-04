
using System;
using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    [Serializable]
    public sealed class Packet : IPacket
    {
        public int T { get; set; }
        public object Data { get; set; }
    }
}
