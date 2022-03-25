﻿using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace The_Project.Accounts
{
    public struct UserId
    {
        public IPAddress Ip { get; }
        public int MinPort { get; }
        public int MaxPort { get; }
        public string AccountId { get; }

        public static Regex Regex { get; } = new(@"\A[A-J]{1,3}a[A-J]{1,3}b[A-J]{1,3}c[A-J]{1,3}ctpr[A-J]{10}[A-Z|a-z]{2,3}$");

        public string Id { get; }

        public UserId(IPAddress ip, int minPort, int maxPort, string accountId)
        {
            char[] alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            Ip = ip;
            MinPort = minPort;
            MaxPort = maxPort;
            AccountId = accountId;

            #region userId

            #region ipConversionToLetters

            string ipConverted = string.Empty;
            string[] octets = ip.ToString().Split(".");
            int pos = 0;
            foreach (string octet in octets)
            {
                ipConverted += octet.Select(x => alphabet[int.Parse(x.ToString())]);
                if (alphabet[pos] == 'D')
                {
                    continue;
                }

                ipConverted += alphabet[pos].ToString().ToLower();
                pos++;
            }

            #endregion ipConversionToLetters

            #region portConversionToLetters

            char[] minPortArr = minPort.ToString().ToCharArray();
            string minPortStr = string.Empty;
            char[] maxPortArr = maxPort.ToString().ToCharArray();
            string maxPortStr = string.Empty;

            minPortStr += minPortArr.Select(x => alphabet[int.Parse(x.ToString())]);
            maxPortStr += maxPortArr.Select(x => alphabet[int.Parse(x.ToString())]);

            #endregion portConversionToLetters

            Id = $"{ipConverted}ctpr{minPortStr}{maxPortStr}{accountId}";

            #endregion userId
        }

        public UserId(string userIdStr)
        {
            Id = userIdStr;

            char[] alphabet = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            #region IPAddress

            string ip = userIdStr.Split("ctpr")[0];
            string octetOne = ip.Split("a")[0];
            string octetTwo = ip.Split("a")[1].Split("b")[0];
            string octetThree = ip.Split("b")[1].Split("c")[0];
            string octetFour = ip.Split("c")[1];

            octetOne = string.Concat(octetOne.ToCharArray().Select(x => Array.IndexOf(alphabet, x).ToString()));
            octetTwo = string.Concat(octetTwo.ToCharArray().Select(x => Array.IndexOf(alphabet, x).ToString()));
            octetThree = string.Concat(octetThree.ToCharArray().Select(x => Array.IndexOf(alphabet, x).ToString()));
            octetFour = string.Concat(octetFour.ToCharArray().Select(x => Array.IndexOf(alphabet, x).ToString()));

            byte[] octetBytes = { (byte)int.Parse(octetOne), (byte)int.Parse(octetTwo), (byte)int.Parse(octetThree), (byte)int.Parse(octetFour) };

            #endregion IPAddress

            Ip = new IPAddress(octetBytes);

            #region PortRange

            string ctpr = userIdStr.Split("ctpr")[1][..10];
            string minPortStr = ctpr[..5];
            string maxPortStr = ctpr[5..10];

            minPortStr = string.Concat(minPortStr.ToCharArray().Select(x => Array.IndexOf(alphabet, x).ToString()));
            maxPortStr = string.Concat(maxPortStr.ToCharArray().Select(x => Array.IndexOf(alphabet, x).ToString()));

            #endregion PortRange

            MinPort = int.Parse(minPortStr);
            MaxPort = int.Parse(maxPortStr);

            AccountId = userIdStr.Split("ctpr")[1].Substring(10, 3);
        }
    }
}