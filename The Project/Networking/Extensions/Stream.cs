﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using The_Project.Networking.Packets.Interfaces;

namespace The_Project.Networking.Extensions
{
    internal static class NetworkExtensions
    {
        internal static void WriteData(this NetworkStream networkStream, IPacket packet)
        {
            List<byte> packetBytes = JsonSerializer.SerializeToUtf8Bytes(packet).ToList();
            packetBytes.Insert(0, Encoding.UTF8.GetBytes("$")[0]);
            networkStream.Write(packetBytes.ToArray(), 0, packetBytes.Count);
        }

        internal static Task WriteDataAsync(this NetworkStream networkStream, IPacket packet)
        {
            List<byte> packetBytes = JsonSerializer.SerializeToUtf8Bytes(packet).ToList();
            packetBytes.Insert(0, Encoding.UTF8.GetBytes("$")[0]);
            return networkStream.WriteAsync(packetBytes.ToArray(), 0, packetBytes.Count);
        }
    }
}