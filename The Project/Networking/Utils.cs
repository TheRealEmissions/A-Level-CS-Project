using System.Linq;
using System.Net;

namespace The_Project.Networking
{
    internal static class Utils
    {
        public static IPAddress GetLocalIpAddress()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(static x =>
                x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
        }
    }
}