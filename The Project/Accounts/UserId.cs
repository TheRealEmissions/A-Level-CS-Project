using System.Diagnostics;
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
            char[] Alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            this.IP = IP;
            this.MinPort = MinPort;
            this.MaxPort = MaxPort;
            this.AccountId = AccountId;
            Regex = new(@"/[A-I]{1,3}a[A-I]{1,3}b[A-I]{1,3}c[A-I]{1,3}ctpr[A-J]{10}[A-Z|a-z]{2}/g");

            #region userId

            #region ipConversionToLetters
            string ipConverted = string.Empty;
            string[] octets = IP.ToString().Split(".");
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
            #endregion ipConversionToLetters

            #region portConversionToLetters
            char[] minPortArr = MinPort.ToString().ToCharArray();
            string minPortStr = string.Empty;
            char[] maxPortArr = MaxPort.ToString().ToCharArray();
            string maxPortStr = string.Empty;
            foreach (char num in minPortArr)
            {
                int n = int.Parse(num.ToString());
                minPortStr += Alphabet[n];
            }
            foreach (char num in maxPortArr)
            {
                int n = int.Parse(num.ToString());
                maxPortStr += Alphabet[n];
            }

            #endregion portConversionToLetters
            this.Id = $"{ipConverted}ctpr{minPortStr}{maxPortStr}{AccountId}";

            #endregion userId
        }
    }
}