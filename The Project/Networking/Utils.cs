﻿using System.Linq;
using System.Net;

namespace The_Project.Networking
{
    public class Utils
    {
        public static IPAddress GetLocalIpAddress()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }
    }
}