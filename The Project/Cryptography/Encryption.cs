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

        public PublicKey(BigInteger n, BigInteger? e = null)
        {
            this.n = n;
            this.e = e ?? 65537;
        }
    }

    public struct PrivateKey
    {
        public BigInteger n;
        public BigInteger d;

        public PrivateKey(BigInteger n, BigInteger d)
        {
            this.n = n;
            this.d = d;
        }
    }

    public static class EncryptionExtensions
    {
        public static string Encrypt(this string s, PublicKey Key)
        {
            byte[] StringBytes = Encoding.UTF8.GetBytes(s.ToCharArray()).ToArray();
            BigInteger[] Numbers = StringBytes.Select(x => (BigInteger)x).ToArray();
            BigInteger[] CipheredNumbers = Numbers.Select(x => BigInteger.ModPow(x, Key.e, Key.n)).ToArray();
            string[] Ciphered = CipheredNumbers.Select(x => x.ToString("X")).ToArray();
            string Cipher = string.Join('-', Ciphered);
            return Cipher;
        }

        public static string Decrypt(this string s, PrivateKey Key)
        {
            string[] Ciphered = s.Split('-');
            BigInteger[] CipheredNumbers = Ciphered.Select(x => BigInteger.Parse(x, System.Globalization.NumberStyles.AllowHexSpecifier)).ToArray();
            BigInteger[] Numbers = CipheredNumbers.Select(x => BigInteger.ModPow(x, Key.d, Key.n)).ToArray();
            byte[] CharBytes = Numbers.Select(x => (byte)x).ToArray();
            char[] Characters = Encoding.UTF8.GetChars(CharBytes);
            return new string(Characters);
        }
    }

    public class Encryption
    {

        private readonly BigInteger x;
        private readonly BigInteger y;
        private readonly BigInteger n;

        private readonly BigInteger phi;

        // find e such that e > 1, e < phi
        // e is co-prime to phi
        private readonly BigInteger e = 65537;//new BigInteger(2).GetCoprime(phi);
        private readonly BigInteger d;

        public readonly PublicKey PublicKey;
        public readonly PrivateKey PrivateKey;

        public Encryption()
        {
            x = GeneratePrime();
            y = GeneratePrime();
            n = x * y;
            phi = (x - 1) * (y - 1);
            d = GetD();

            PublicKey = new(n, e);
            PrivateKey = new(n, d);
        }

        public Encryption(PublicKey Public, PrivateKey Private)
        {
            PublicKey = Public;
            PrivateKey = Private;
        }

        private BigInteger GetD()
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

        private BigInteger GeneratePrime(int bits = 1024)
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
    }
}
