using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Accounts
{
    public struct UserId
    {
        public IPAddress IP { get; }
        public int MinPort { get; }
        public int MaxPort { get; }
        public string AccountId { get; }

        public UserId(IPAddress IP, int MinPort, int MaxPort, string AccountId)
        {
            this.IP = IP;
            this.MinPort = MinPort;
            this.MaxPort = MaxPort;
            this.AccountId = AccountId;
        }
    }
}
