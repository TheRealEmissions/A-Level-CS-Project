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
        // N is such a number in the public key which is two arbitrarily large random prime numbers multiplied together
        public BigInteger N { get; }

        // E is the exponent of the public key for which the data needing to be encrypted should be raised to
        public BigInteger E { get; }

        /**
         * Creation of public key
         *
         * Parameter n represents N (two random prime numbers multiplied together)
         * Parameter e is the exponent which is default 65537 as standard in RSA PKCS
         */

        public PublicKey(BigInteger n, BigInteger? e = null)
        {
            N = n;
            E = e ?? 65537;
        }
    }

    public struct PrivateKey
    {
        // Similarly to the public key, N is the same two arbitrarily large random prime numbers multiplied together
        public BigInteger N { get; }

        // D is the "secret exponent" of the exponent of which the encrypted data is raised to to return it to its original state
        public BigInteger D { get; }

        /**
         * Creation of private key
         *
         * Parameter n represents N (two random prime numbers multiplied together)
         * Parameter d represents the "secret exponent"
         */

        public PrivateKey(BigInteger n, BigInteger d)
        {
            N = n;
            D = d;
        }
    }

    /**
     * Class used to add "Encrypt" and "Decrypt" extension methods to string
     */

    public static class EncryptionExtensions
    {
        public static string Encrypt(this string s, PublicKey Key)
        {
            // Convert string to array of bytes using UTF8
            byte[]? StringBytes = Encoding.UTF8.GetBytes(s);

            /**
             * PARALLELISED FOR EFFICIENCY USING PLINQ
             * Ordered to ensure encrypted string is returned in the correct order
             *
             * For each byte in the byte array (StringBytes) converted to an unsigned 32 bit integer, it will be raised to the power E and modulus with N
             * and converted to hexidecimal string
             */
            IEnumerable<string> Ciphered = StringBytes.AsParallel().AsOrdered().Select(x => BigInteger.ModPow((uint)x, Key.E, Key.N).ToString("X"));
            //string Cipher = Ciphered.ParallelJoin('-');

            // Each hexidecimal will then by joined by "-" and returned from the method as a string
            return string.Join('-', Ciphered);
        }

        public static string Decrypt(this string s, PrivateKey Key)
        {
            /**
             * PARALLELISED FOR EFFICIENCY USING PLINQ
             * Ordered to ensure decrypted string is returned in the correct order
             *
             * String provided is split by "-" which converts it to a string array
             * In parallel, each element is parsed by BigInteger from hexidecimal into a number
             * Raised to the power D (secret exponent) and modulus with N
             * This number is then converted to a byte (the original byte before encryption)
             */
            IEnumerable<byte>? CharBytes = s.Split('-').AsParallel().AsOrdered().Select(x => (byte)BigInteger.ModPow(BigInteger.Parse(x, System.Globalization.NumberStyles.AllowHexSpecifier), Key.D, Key.N));
            // Using the same encoding UTF8, the byte array returned is converted to a string
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