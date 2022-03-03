using System;
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

        public static Regex Regex { get; } = new(@"\A[A-J]{1,3}a[A-J]{1,3}b[A-J]{1,3}c[A-J]{1,3}ctpr[A-J]{10}[A-Z|a-z]{2,3}$");

        public string Id { get; }

        public UserId(IPAddress IP, int MinPort, int MaxPort, string AccountId)
        {
            char[] Alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            this.IP = IP;
            this.MinPort = MinPort;
            this.MaxPort = MaxPort;
            this.AccountId = AccountId;

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

        public UserId(string UserIdStr)
        {
            this.Id = UserIdStr;

            char[] Alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            #region IPAddress

            string IP = UserIdStr.Split("ctpr")[0];
            string OctetOne = IP.Split("a")[0];
            string OctetTwo = IP.Split("a")[1].Split("b")[0];
            string OctetThree = IP.Split("b")[1].Split("c")[0];
            string OctetFour = IP.Split("c")[1];

            OctetOne = string.Concat(OctetOne.ToCharArray().Select(x => Array.IndexOf(Alphabet, x).ToString()));
            OctetTwo = string.Concat(OctetTwo.ToCharArray().Select(x => Array.IndexOf(Alphabet, x).ToString()));
            OctetThree = string.Concat(OctetThree.ToCharArray().Select(x => Array.IndexOf(Alphabet, x).ToString()));
            OctetFour = string.Concat(OctetFour.ToCharArray().Select(x => Array.IndexOf(Alphabet, x).ToString()));

            byte[] OctetBytes = new byte[4] { (byte)int.Parse(OctetOne), (byte)int.Parse(OctetTwo), (byte)int.Parse(OctetThree), (byte)int.Parse(OctetFour) };

            #endregion IPAddress

            this.IP = new(OctetBytes);

            #region PortRange

            string CTPR = UserIdStr.Split("ctpr")[1].Substring(0, 10);
            string MinPortStr = CTPR.Substring(0, 5);
            string MaxPortStr = CTPR.Substring(5, 5);

            MinPortStr = string.Concat(MinPortStr.ToCharArray().Select(x => Array.IndexOf(Alphabet, x).ToString()));
            MaxPortStr = string.Concat(MaxPortStr.ToCharArray().Select(x => Array.IndexOf(Alphabet, x).ToString()));

            #endregion PortRange

            this.MinPort = int.Parse(MinPortStr);
            this.MaxPort = int.Parse(MaxPortStr);

            this.AccountId = UserIdStr.Split("ctpr")[1].Substring(10, 3);
        }
    }
}