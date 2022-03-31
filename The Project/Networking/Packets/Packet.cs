using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Packets
{
    public class Packet : IPacket
    {
        public int T { get; set; }
        public object Data { get; set; }
    }
}
