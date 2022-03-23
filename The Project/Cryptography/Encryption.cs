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
        public static string Encrypt(this string s, PublicKey key)
        {
            // Convert string to array of bytes using UTF8
            byte[] stringBytes = Encoding.UTF8.GetBytes(s);

            /**
             * PARALLELISED FOR EFFICIENCY USING PLINQ
             * Ordered to ensure encrypted string is returned in the correct order
             *
             * For each byte in the byte array (StringBytes) converted to an unsigned 32 bit integer, it will be raised to the power E and modulus with N
             * and converted to hexidecimal string
             */
            IEnumerable<string> ciphered = stringBytes.AsParallel().AsOrdered().Select(x => BigInteger.ModPow((uint)x, key.E, key.N).ToString("X"));
            //string Cipher = Ciphered.ParallelJoin('-');

            // Each hexidecimal will then by joined by "-" and returned from the method as a string
            return string.Join('-', ciphered);
        }

        public static string Decrypt(this string s, PrivateKey key)
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
            IEnumerable<byte> charBytes = s.Split('-').AsParallel().AsOrdered().Select(x => (byte)BigInteger.ModPow(BigInteger.Parse(x, System.Globalization.NumberStyles.AllowHexSpecifier), key.D, key.N));
            // Using the same encoding UTF8, the byte array returned is converted to a string
            return Encoding.UTF8.GetString(charBytes.ToArray());
        }
    }

    public class Encryption
    {

        private readonly BigInteger _phi;

        // find _eInteger such that _eInteger > 1, _eInteger < _phi
        // _eInteger is co-prime to _phi
        private readonly BigInteger _eInteger = 65537;//new BigInteger(2).GetCoprime(_phi);

        public PublicKey PublicKeyKey { get; }
        public PrivateKey PrivateKeyKey { get; }

        public Encryption()
        {
            BigInteger xInteger = GeneratePrime();
            BigInteger yInteger = GeneratePrime();
            BigInteger nInteger = xInteger * yInteger;
            _phi = (xInteger - 1) * (yInteger - 1);
            BigInteger dInteger = GetD();

            PublicKeyKey = new PublicKey(nInteger, _eInteger);
            PrivateKeyKey = new PrivateKey(nInteger, dInteger);
        }

        public Encryption(PublicKey publicKey, PrivateKey privateKey)
        {
            PublicKeyKey = publicKey;
            PrivateKeyKey = privateKey;
        }

        private BigInteger GetD()
        {
            BigInteger d = (1 + _phi) / _eInteger;
            int i = 1;
            while (_eInteger * d % _phi != (1 % _phi))
            {
                d = (1 + ((i + 1) * _phi)) / _eInteger;
                i++;
            }
            return d;
        }

        private static BigInteger GeneratePrime(int bits = 1024)
        {
            byte[] buffer = new byte[bits / 8];
            new Random().NextBytes(buffer);
            BitArray bitArray = new(buffer);
            bitArray.Set(0, true);
            bitArray.Set(bits - 1, true);
            bitArray.CopyTo(buffer, 0);
            BigInteger prime = new(buffer, true);
            return prime.IsProbablyPrime() ? prime : GeneratePrime(bits);
        }
    }
}