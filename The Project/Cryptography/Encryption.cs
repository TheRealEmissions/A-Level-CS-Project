using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using The_Project.Extensions;

#nullable enable

namespace The_Project.Cryptography
{
    public struct PublicKey
    {
        public BigInteger N { get; }
        public BigInteger E { get; }

        public PublicKey(BigInteger n, BigInteger? e = null)
        {
            N = n;
            E = e ?? 65537;
        }
    }

    public struct PrivateKey
    {
        public BigInteger N { get; }
        public BigInteger D { get; }

        public PrivateKey(BigInteger n, BigInteger d)
        {
            N = n;
            D = d;
        }
    }

    public static class EncryptionExtensions
    {
        public static string Encrypt(this string s, PublicKey Key)
        {
            byte[]? StringBytes = Encoding.UTF8.GetBytes(s);
            IEnumerable<string> Ciphered = StringBytes.AsParallel().AsOrdered().Select(x => BigInteger.ModPow((uint)x, Key.E, Key.N).ToString("X"));
            //string Cipher = Ciphered.ParallelJoin('-');
            return string.Join('-', Ciphered);
        }

        public static string Decrypt(this string s, PrivateKey Key)
        {
            IEnumerable<byte>? CharBytes = s.Split('-').AsParallel().AsOrdered().Select(x => (byte)BigInteger.ModPow(BigInteger.Parse(x, System.Globalization.NumberStyles.AllowHexSpecifier), Key.D, Key.N));
            //IEnumerable<byte>? CharBytes = CipheredNumbers.Select(x => (byte)BigInteger.ModPow(x, Key.d, Key.n));
            return Encoding.UTF8.GetString(CharBytes.ToArray());
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

        public PublicKey PublicKey { get; }
        public PrivateKey PrivateKey { get; }

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
            while (e * d % phi != (1 % phi))
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