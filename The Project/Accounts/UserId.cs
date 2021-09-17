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
        public IPAddress IP;
        public int MinPort;
        public int MaxPort;
        public string AccountId;

        public UserId(IPAddress IP, int MinPort, int MaxPort, string AccountId)
        {
            this.IP = IP;
            this.MinPort = MinPort;
            this.MaxPort = MaxPort;
            this.AccountId = AccountId;
        }
    }
}
