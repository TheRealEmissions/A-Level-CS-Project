using System;
using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    [Serializable]
    internal sealed class ExceptionPacket : IPacket
    {

        public int T { get; set; } = 3;
        public int E { get; set; }
        public string S { get; set; }
    }
}
