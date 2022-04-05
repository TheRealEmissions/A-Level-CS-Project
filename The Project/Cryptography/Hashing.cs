using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using The_Project.Extensions;

namespace The_Project.Cryptography
{
    /**
     * SHA-256 IMPLEMENTATION
     *
     * OUTPUTS 64 BIT STRING for ANY LENGTH input
     * IRREVERSIBLE
     */
    internal sealed class Hashing
    {
        private readonly MainWindow _mainWindow;

        internal Hashing(MainWindow window)
        {
            _mainWindow = window;
        }

        private uint _primeHash0 = 0x6a09e667;
        private uint _primeHash1 = 0xbb67ae85;
        private uint _primeHash2 = 0x3c6ef372;
        private uint _primeHash3 = 0xa54ff53a;
        private uint _primeHash4 = 0x510e527f;
        private uint _primeHash5 = 0x9b05688c;
        private uint _primeHash6 = 0x1f83d9ab;
        private uint _primeHash7 = 0x5be0cd19;

        private readonly uint[] _roundConstants =
        {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
        };

        public string Hash(string str)
        {
            // implementation of SHA256 hashing

            // turn string into bytes (based on UTF8)
            BitArray bytesArray = new(Encoding.UTF8.GetBytes(str));

            // ***
            // pad string
            // ***
            bytesArray = Pad(bytesArray);

            _mainWindow.Debug($"PADDED ARR LENGTH: {bytesArray.Length}");

            // ***
            // break into 512 bit chunks
            // ***
            List<BitArray> chunkedArr = SplitIntoChunks(bytesArray);

            // ***
            // create message schedule
            // SPLITS INTO 32 bit chunks (16 originally, then adds 48 initalised to 0 for 64 bit)
            // ***

            foreach (BitArray chunk in chunkedArr)
            {
                // splits into 32 bit chunks (16 original entries)
                List<BitArray> chunk32Bit = SplitIntoChunks(chunk, 32); // 16 entries
                // adds 48 more 32 bit chunks (initialised to 0)
                chunk32Bit.AddRange(Enumerable.Repeat(new BitArray(32), 48));
                // loop to extend original 16 entries into the other 48 entries
                for (int j = 16; j < 64; j++)
                {
                    // logical bitwise operations on temporary variables for extension
                    BitArray s0 = chunk32Bit[j - 15].SafeRightRotate(7)
                        .SafeXor(chunk32Bit[j - 15].SafeRightRotate(18))
                        .SafeXor(chunk32Bit[j - 15].SafeRightShift(3));
                    BitArray s1 = chunk32Bit[j - 2].SafeRightRotate(17)
                        .SafeXor(chunk32Bit[j - 2].SafeRightRotate(19))
                        .SafeXor(chunk32Bit[j - 2].SafeRightShift(10));
                    // re-assign entry after bitwise operations
                    chunk32Bit[j] = chunk32Bit[j - 16].Add(s0)
                        .Add(chunk32Bit[j - 7])
                        .Add(s1);
                }

                BitArray primeHash0Bits = new(BitConverter.GetBytes(_primeHash0));
                BitArray primeHash1Bits = new(BitConverter.GetBytes(_primeHash1));
                BitArray primeHash2Bits = new(BitConverter.GetBytes(_primeHash2));
                BitArray primeHash3Bits = new(BitConverter.GetBytes(_primeHash3));
                BitArray primeHash4Bits = new(BitConverter.GetBytes(_primeHash4));
                BitArray primeHash5Bits = new(BitConverter.GetBytes(_primeHash5));
                BitArray primeHash6Bits = new(BitConverter.GetBytes(_primeHash6));
                BitArray primeHash7Bits = new(BitConverter.GetBytes(_primeHash7));

                for (int j = 0; j < 64; j++)
                {
                    BitArray s1 = primeHash4Bits.SafeRightRotate(6)
                        .SafeXor(primeHash4Bits.SafeRightRotate(11))
                        .SafeXor(primeHash4Bits.SafeRightRotate(25));
                    BitArray ch = primeHash4Bits.SafeAnd(primeHash5Bits)
                        .SafeXor(primeHash4Bits.SafeNot().SafeAnd(primeHash6Bits));
                    BitArray temp1 = primeHash7Bits.Add(s1)
                        .Add(ch)
                        .Add(new BitArray(BitConverter.GetBytes(_roundConstants[j])))
                        .Add(chunk32Bit[j]);
                    BitArray s0 = primeHash0Bits.SafeRightRotate(2)
                        .SafeXor(primeHash0Bits.SafeRightRotate(13))
                        .SafeXor(primeHash0Bits.SafeRightRotate(22));
                    BitArray maj = primeHash0Bits.SafeAnd(primeHash1Bits)
                        .SafeXor(primeHash0Bits.SafeAnd(primeHash2Bits))
                        .SafeXor(primeHash1Bits.SafeAnd(primeHash2Bits));
                    BitArray temp2 = s0.Add(maj);

                    primeHash7Bits = primeHash6Bits;
                    primeHash6Bits = primeHash5Bits;
                    primeHash5Bits = primeHash4Bits;
                    primeHash4Bits = primeHash3Bits.Add(temp1);
                    primeHash3Bits = primeHash2Bits;
                    primeHash2Bits = primeHash1Bits;
                    primeHash1Bits = primeHash0Bits;
                    primeHash0Bits = temp1.Add(temp2);
                }

                BitArray temph0 = new BitArray(BitConverter.GetBytes(_primeHash0)).Add(primeHash0Bits);
                BitArray temph1 = new BitArray(BitConverter.GetBytes(_primeHash1)).Add(primeHash1Bits);
                BitArray temph2 = new BitArray(BitConverter.GetBytes(_primeHash2)).Add(primeHash2Bits);
                BitArray temph3 = new BitArray(BitConverter.GetBytes(_primeHash3)).Add(primeHash3Bits);
                BitArray temph4 = new BitArray(BitConverter.GetBytes(_primeHash4)).Add(primeHash4Bits);
                BitArray temph5 = new BitArray(BitConverter.GetBytes(_primeHash5)).Add(primeHash5Bits);
                BitArray temph6 = new BitArray(BitConverter.GetBytes(_primeHash6)).Add(primeHash6Bits);
                BitArray temph7 = new BitArray(BitConverter.GetBytes(_primeHash7)).Add(primeHash7Bits);

                _primeHash0 = temph0.ToUInt32();
                _primeHash1 = temph1.ToUInt32();
                _primeHash2 = temph2.ToUInt32();
                _primeHash3 = temph3.ToUInt32();
                _primeHash4 = temph4.ToUInt32();
                _primeHash5 = temph5.ToUInt32();
                _primeHash6 = temph6.ToUInt32();
                _primeHash7 = temph7.ToUInt32();
            }

            return string.Concat(new[]
                {
                    _primeHash0, _primeHash1, _primeHash2, _primeHash3, _primeHash4, _primeHash5, _primeHash6,
                    _primeHash7
                }.AsParallel()
                .AsOrdered()
                .WithDegreeOfParallelism(Environment.ProcessorCount)
                .Select(static x => x.ToString("X").PadLeft(8, '0')));
        }

        private static List<BitArray> SplitIntoChunks(BitArray arr, int bits = 512)
        {
            byte[] buffer = new byte[arr.Length / 8];
            arr.CopyTo(buffer, 0);

            List<BitArray> bitArrayList = new();

            int originalLength = arr.Length / 8;
            for (int i = 0; i < originalLength; i += bits / 8)
            {
                BitArray bitArray = new(buffer[i..(i + (bits / 8))]);
                bitArrayList.Add(bitArray);
            }

            return bitArrayList;
        }

        private static BitArray Pad(BitArray arr)
        {
            int originalLength = arr.Length;

            arr.Length += 1;
            arr.Set(arr.Length - 1, true);

            int i = 0;
            while ((originalLength + 1 + i + 64) % 512 != 0)
            {
                arr.Length += 1;
                arr.Set(arr.Length - 1, false);
                i++;
            }

            byte[] int64Bytes = BitConverter.GetBytes(Convert.ToUInt64(originalLength));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(int64Bytes);
            }

            BitArray messageLengthArr = new(int64Bytes);

            foreach (bool b in messageLengthArr)
            {
                arr.Length += 1;
                arr.Set(arr.Length - 1, b);
            }

            return arr;
        }
    }
}