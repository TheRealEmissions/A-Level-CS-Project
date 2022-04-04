using System;
using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    [Serializable]
    public sealed class AccountIdVerificationPacket : IPacket
    {
        public int T { get; set; }
        public string A { get; set; }

    }
}
