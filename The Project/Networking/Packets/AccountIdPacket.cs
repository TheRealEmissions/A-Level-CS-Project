using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    [Serializable]
    public sealed class AccountIdPacket : IPacket
    {
        public int T { get; set; } = 5;
        public string A { get; set; }
    }
}
