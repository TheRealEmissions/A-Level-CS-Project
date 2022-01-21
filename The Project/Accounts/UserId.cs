using System.Diagnostics;
using System.Linq;
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

        public string Id { get; }

        public UserId(IPAddress IP, int MinPort, int MaxPort, string AccountId)
        {
            char[] Alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

            this.IP = IP;
            this.MinPort = MinPort;
            this.MaxPort = MaxPort;
            this.AccountId = AccountId;

            #region userId

            string ipConverted = string.Empty;
            var octets = IP.ToString().Split(".");
            int pos = 0;
            foreach (string octet in octets)
            {
                foreach (char number in octet)
                {
                    int num = int.Parse(number.ToString());
                    ipConverted += Alphabet[num];
                }
                if (Alphabet[pos] == 'D')
                {
                    continue;
                }

                ipConverted += Alphabet[pos].ToString().ToLower();
                pos++;
            }
            Debug.WriteLine(ipConverted);
            this.Id = $"{ipConverted}ctpr{MinPort}{MaxPort}{AccountId}";

            #endregion
            Regex = new(@"/[A-I]{1,3}a[A-I]{1,3}b[A-I]{1,3}c[A-I]{1,3}ctpr[A-J]{10}[A-Z|a-z]{2}/g");
        }
    }
}