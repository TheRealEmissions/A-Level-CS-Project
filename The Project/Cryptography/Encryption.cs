using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using The_Project.Extensions;

#nullable enable
/**
 * IMPLEMENTATION OF RSA ENCRYPTION
 * HEXADECIMAL CONVERSION
 */
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
        public static string Encrypt(this string s, PublicKey Key, MainWindow? window = null)
        {
            byte[]? StringBytes = Encoding.UTF8.GetBytes(s);

            DateTime startNumbers = DateTime.Now;
            IEnumerable<uint>? Numbers = StringBytes.AsParallel().AsOrdered().Select(x => (uint)x);
            DateTime endNumbers = DateTime.Now;
            Debug.WriteLine($"Numbers -> {(endNumbers - startNumbers).TotalMilliseconds}ms");

            StringBytes = null;
            DateTime startCN = DateTime.Now;
            IEnumerable<BigInteger>? CipheredNumbers = Numbers.AsParallel().AsOrdered().Select(x => BigInteger.ModPow(x, Key.e, Key.n));
            DateTime endCN = DateTime.Now;
            Debug.WriteLine($"CN -> {(endCN - startCN).TotalMilliseconds}ms");

            // find more efficient way to convert BigInteger (too slow right now)
            Numbers = null;
            DateTime startCiphered = DateTime.Now;
            string[] Ciphered = CipheredNumbers.AsParallel().AsOrdered().Select(x => x.ToString("X")).ToArray();
            DateTime endCiphered = DateTime.Now;
            Debug.WriteLine($"Ciphered -> {(endCiphered - startCiphered).TotalMilliseconds}ms");

            DateTime startCipher = DateTime.Now;
            string Cipher = Ciphered.ParallelJoin('-');
            return Cipher; //string.Join('-', Ciphered);
        }

        public static string Decrypt(this string s, PrivateKey Key)
        {
            IEnumerable<byte>? CharBytes = s.Split('-').AsParallel().AsOrdered().WithDegreeOfParallelism(Environment.ProcessorCount).Select(x => (byte)BigInteger.ModPow(BigInteger.Parse(x, System.Globalization.NumberStyles.AllowHexSpecifier), Key.d, Key.n));
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
