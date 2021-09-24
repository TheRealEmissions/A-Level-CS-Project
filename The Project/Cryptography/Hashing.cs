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

    public class Hashing
    {
        private MainWindow mainWindow;

        public Hashing(MainWindow window)
        {
            mainWindow = window;
        }

        private uint PrimeHash0 = 0x6a09e667;
        private uint PrimeHash1 = 0xbb67ae85;
        private uint PrimeHash2 = 0x3c6ef372;
        private uint PrimeHash3 = 0xa54ff53a;
        private uint PrimeHash4 = 0x510e527f;
        private uint PrimeHash5 = 0x9b05688c;
        private uint PrimeHash6 = 0x1f83d9ab;
        private uint PrimeHash7 = 0x5be0cd19;

        private uint[] RoundConstants = new uint[64] {0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
   0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3, 0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
   0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc, 0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
   0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7, 0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
   0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13, 0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
   0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3, 0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
   0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5, 0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
   0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208, 0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2};

        public string Hash(string str)
        {
            // implementation of SHA256 hashing

            // turn string into bytes
            BitArray StringToBytesArray = new(Encoding.UTF8.GetBytes(str));

            // ***
            // pad string
            // ***
            StringToBytesArray = Pad(StringToBytesArray);

            mainWindow.Debug($"PADDED ARR LENGTH: {StringToBytesArray.Length}");

            // ***
            // break into 512 bit chunks
            // ***
            List<BitArray> ChunkedArr = SplitIntoChunks(StringToBytesArray);

            // ***
            // create message schedule
            // SPLITS INTO 32 bit chunks (16 originally, then adds 48 initalised to 0 for 64 bit)
            // ***



            foreach (BitArray chunk in ChunkedArr)
            {
                List<BitArray> chunk32bit = SplitIntoChunks(chunk, 32); // 16 entries
                chunk32bit.AddRange(Enumerable.Repeat(new BitArray(32), 48));
                /*                for (int j = 0; j < 48; j++)
                                {
                                    chunk32bit.Add(new BitArray(32)); // add 48 more to bring to 64 (256 bytes)
                                }*/

                for (int j = 16; j < 64; j++)
                {
                    BitArray s0 = chunk32bit[j - 15].SafeRightRotate(7).SafeXor(chunk32bit[j - 15].SafeRightRotate(18)).SafeXor(chunk32bit[j - 15].SafeRightShift(3));
                    BitArray s1 = chunk32bit[j - 2].SafeRightRotate(17).SafeXor(chunk32bit[j - 2].SafeRightRotate(19)).SafeXor(chunk32bit[j - 2].SafeRightShift(10));
                    chunk32bit[j] = chunk32bit[j - 16].Add(s0).Add(chunk32bit[j - 7]).Add(s1);
                }

                BitArray PrimeHash0Bits = new BitArray(BitConverter.GetBytes(PrimeHash0));
                BitArray PrimeHash1Bits = new BitArray(BitConverter.GetBytes(PrimeHash1));
                BitArray PrimeHash2Bits = new BitArray(BitConverter.GetBytes(PrimeHash2));
                BitArray PrimeHash3Bits = new BitArray(BitConverter.GetBytes(PrimeHash3));
                BitArray PrimeHash4Bits = new BitArray(BitConverter.GetBytes(PrimeHash4));
                BitArray PrimeHash5Bits = new BitArray(BitConverter.GetBytes(PrimeHash5));
                BitArray PrimeHash6Bits = new BitArray(BitConverter.GetBytes(PrimeHash6));
                BitArray PrimeHash7Bits = new BitArray(BitConverter.GetBytes(PrimeHash7));

                for (int j = 0; j < 64; j++)
                {
                    BitArray S1 = PrimeHash4Bits.SafeRightRotate(6).SafeXor(PrimeHash4Bits.SafeRightRotate(11)).SafeXor(PrimeHash4Bits.SafeRightRotate(25));
                    BitArray ch = PrimeHash4Bits.SafeAnd(PrimeHash5Bits).SafeXor(PrimeHash4Bits.SafeNot().SafeAnd(PrimeHash6Bits));
                    BitArray temp1 = PrimeHash7Bits.Add(S1).Add(ch).Add(new BitArray(BitConverter.GetBytes(RoundConstants[j]))).Add(chunk32bit[j]);
                    BitArray S0 = PrimeHash0Bits.SafeRightRotate(2).SafeXor(PrimeHash0Bits.SafeRightRotate(13)).SafeXor(PrimeHash0Bits.SafeRightRotate(22));
                    BitArray maj = PrimeHash0Bits.SafeAnd(PrimeHash1Bits).SafeXor(PrimeHash0Bits.SafeAnd(PrimeHash2Bits)).SafeXor(PrimeHash1Bits.SafeAnd(PrimeHash2Bits));
                    BitArray temp2 = S0.Add(maj);

                    PrimeHash7Bits = PrimeHash6Bits;
                    PrimeHash6Bits = PrimeHash5Bits;
                    PrimeHash5Bits = PrimeHash4Bits;
                    PrimeHash4Bits = PrimeHash3Bits.Add(temp1);
                    PrimeHash3Bits = PrimeHash2Bits;
                    PrimeHash2Bits = PrimeHash1Bits;
                    PrimeHash1Bits = PrimeHash0Bits;
                    PrimeHash0Bits = temp1.Add(temp2);
                }

                BitArray temph0 = new BitArray(BitConverter.GetBytes(PrimeHash0)).Add(PrimeHash0Bits);
                BitArray temph1 = new BitArray(BitConverter.GetBytes(PrimeHash1)).Add(PrimeHash1Bits);
                BitArray temph2 = new BitArray(BitConverter.GetBytes(PrimeHash2)).Add(PrimeHash2Bits);
                BitArray temph3 = new BitArray(BitConverter.GetBytes(PrimeHash3)).Add(PrimeHash3Bits);
                BitArray temph4 = new BitArray(BitConverter.GetBytes(PrimeHash4)).Add(PrimeHash4Bits);
                BitArray temph5 = new BitArray(BitConverter.GetBytes(PrimeHash5)).Add(PrimeHash5Bits);
                BitArray temph6 = new BitArray(BitConverter.GetBytes(PrimeHash6)).Add(PrimeHash6Bits);
                BitArray temph7 = new BitArray(BitConverter.GetBytes(PrimeHash7)).Add(PrimeHash7Bits);

                PrimeHash0 = temph0.ToUInt32();
                PrimeHash1 = temph1.ToUInt32();
                PrimeHash2 = temph2.ToUInt32();
                PrimeHash3 = temph3.ToUInt32();
                PrimeHash4 = temph4.ToUInt32();
                PrimeHash5 = temph5.ToUInt32();
                PrimeHash6 = temph6.ToUInt32();
                PrimeHash7 = temph7.ToUInt32();
            }

            return string.Concat((new uint[8] { PrimeHash0, PrimeHash1, PrimeHash2, PrimeHash3, PrimeHash4, PrimeHash5, PrimeHash6, PrimeHash7 }).AsParallel().AsOrdered().WithDegreeOfParallelism(Environment.ProcessorCount).Select(x => x.ToString("X").PadLeft(8, '0')));
        }

        private static List<BitArray> SplitIntoChunks(BitArray arr, int bits = 512)
        {
            int originalLength = arr.Length / 8;

            byte[] Bytes = new byte[arr.Length / 8];
            arr.CopyTo(Bytes, 0);

            List<BitArray> BitArrayList = new();
            for (int i = 0; i < originalLength; i += bits / 8)
            {
                BitArray bitArray = new(Bytes[i..(i + (bits / 8))]);
                BitArrayList.Add(bitArray);
            }
            return BitArrayList;
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

            byte[] Int64Bytes = BitConverter.GetBytes(Convert.ToUInt64(originalLength));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(Int64Bytes);
            }

            BitArray messageLengthArr = new(Int64Bytes);

            foreach (bool b in messageLengthArr)
            {
                arr.Length += 1;
                arr.Set(arr.Length - 1, b);
            }

            return arr;
        }
    }
}