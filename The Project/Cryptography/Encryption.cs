using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using The_Project.Extensions;

namespace The_Project.Cryptography
{
    public struct PublicKey
    {
        public BigInteger n;
        public BigInteger e;

        public PublicKey(BigInteger n, BigInteger e)
        {
            this.n = n;
            this.e = e;
        }
    }

    internal struct PrivateKey
    {
        public BigInteger n;
        public BigInteger d;

        public PrivateKey(BigInteger n, BigInteger d)
        {
            this.n = n;
            this.d = d;
        }
    }

    public static class Encryption
    {
        private static readonly BigInteger x = GeneratePrime();
        private static readonly BigInteger y = GeneratePrime();
        private static readonly BigInteger n = x * y;

        private static readonly BigInteger phi = (x - 1) * (y - 1);

        private static readonly BigInteger e = 65537;//new BigInteger(2).GetCoprime(phi);
        private static readonly BigInteger d = GetD();

        public static readonly PublicKey Public = new(n, e);
        private static readonly PrivateKey Private = new(n, d);

        private static BigInteger GetD()
        {
            BigInteger d = (1 + phi) / e;
            int i = 1;
            while (e*d%phi != (1%phi))
            {
                d = (1 + ((i + 1) * phi)) / e;
                i++;
            }
            return d;
        }

        // find e such that e > 1, e < phi
        // e is co-prime to phi
        private static BigInteger GetCoprime(this BigInteger x, BigInteger y)
        {
            while (x < y)
            {
                if (GreatestCommonDivider(x, y) == 1) break;
                x++;
            }
            return x;
        }

        private static BigInteger GreatestCommonDivider(BigInteger a, BigInteger b)
        {
            BigInteger temp = a % b;
            if (temp == 0) return b;
            return GreatestCommonDivider(b, temp);
        }

        private static BigInteger GeneratePrime(int bits = 1024)
        {
            byte[] Byte = new byte[bits / 8];
            new Random().NextBytes(Byte);
            BitArray bitArray = new(Byte);
            bitArray.Set(0, true);
            bitArray.Set(bits - 1, true);
            bitArray.CopyTo(Byte, 0);
            BigInteger Prime = new(Byte, true);
            return Prime.IsProbablyPrime() ? Prime : GeneratePrime(bits);
        }

        private static bool IsProbablyPrime(this BigInteger n)
        {
            if (n <= 1 || n % 2 == 0) return false;
            if (n == 3) return true;

            BigInteger a = new BigInteger(2).GetCoprime(n);
            BigInteger result = BigInteger.ModPow(a, n - 1, n);
            if (result != 1)
            {
                return false;
            }
            return true;
        }

        public static string Encrypt(this string s, PublicKey Key)
        {
            byte[] StringBytes = Encoding.UTF8.GetBytes(s.ToCharArray()).ToArray();
            BigInteger[] Numbers = StringBytes.Select(x => (BigInteger)x).ToArray();
            BigInteger[] CipheredNumbers = Numbers.Select(x => BigInteger.ModPow(x, Key.e, Key.n)).ToArray();
            string[] Ciphered = CipheredNumbers.Select(x => x.ToString("X")).ToArray();
            string Cipher = string.Join('-', Ciphered);
            return Cipher;
        }

        public static string Decrypt(this string s)
        {
            string[] Ciphered = s.Split('-');
            BigInteger[] CipheredNumbers = Ciphered.Select(x => BigInteger.Parse(x, System.Globalization.NumberStyles.AllowHexSpecifier)).ToArray();
            BigInteger[] Numbers = CipheredNumbers.Select(x => BigInteger.ModPow(x, Private.d, Private.n)).ToArray();
            byte[] CharBytes = Numbers.Select(x => (byte)x).ToArray();
            char[] Characters = Encoding.UTF8.GetChars(CharBytes);
            return new string(Characters);
        }
    }
}
