using System.Net;
using System.Text.RegularExpressions;

namespace The_Project.Accounts
{
    public struct UserId
    {
        public IPAddress IP { get; }
        public int MinPort { get; }
        public int MaxPort { get; }
        public string AccountId { get; }

        public Regex Regex { get; }

        public UserId(IPAddress IP, int MinPort, int MaxPort, string AccountId)
        {
            this.IP = IP;
            this.MinPort = MinPort;
            this.MaxPort = MaxPort;
            this.AccountId = AccountId;
            Regex = new(@"/[A-I]{1,3}a[A-I]{1,3}b[A-I]{1,3}c[A-I]{1,3}ctpr[A-J]{10}[A-Z|a-z]{2}/g");
        }
    }
}