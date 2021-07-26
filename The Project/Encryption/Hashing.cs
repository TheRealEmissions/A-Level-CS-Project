using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace The_Project.Encryption
{
    public class Hashing
    {
        private MainWindow mainWindow;

        public Hashing(MainWindow window)
        {
            mainWindow = window;
        }

        private const uint h0 = 0x6a09e667;
        private const uint h1 = 0xbb67ae85;
        private const uint h2 = 0x3c6ef372;
        private const uint h3 = 0xa54ff53a;
        private const uint h4 = 0x510e527f;
        private const uint h5 = 0x9b05688c;
        private const uint h6 = 0x1f83d9ab;
        private const uint h7 = 0x5be0cd19;

        private uint[] k = new uint[64] {0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5, 0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
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

            string s = "";

            // turn string into bytes
            BitArray arr = new(Encoding.UTF8.GetBytes(str));

            // ***
            // pad string
            // ***
            arr = Pad(arr);

            mainWindow.Output($"PADDED ARR LENGTH: {arr.Length}");

            // ***
            // break into 512 bit chunks
            // ***
            List<BitArray> ChunkedArr = SplitIntoChunks(arr);

            // ***
            // create message schedule
            // ***

            foreach (BitArray chunk in ChunkedArr)
            {
                List<BitArray> chunk32bit = SplitIntoChunks(chunk, 32);
                for (int j = 0; j < 64; j++) chunk32bit.Add(new BitArray(32));
                for (int j = 0; j < 64; j++)
                {
                    BitArray tempBitArray = new BitArray(chunk32bit[j-15]);
                    BitArray s0 = Utils.Bitwise.RightRotate(chunk32bit[j - 15], 7).Xor(Utils.Bitwise.RightRotate(chunk32bit[j - 15], 18)).Xor(chunk32bit[j-15].RightShift(3));
                    BitArray s1 = Utils.Bitwise.RightRotate(chunk32bit[j - 2], 17).Xor(Utils.Bitwise.RightRotate(chunk32bit[j - 2], 19)).Xor(chunk32bit[j - 2].RightShift(10));
                    //chunk32bit[j] = chunk32bit[j - 16] + s0 + chunk32bit[j - 7] + s1;
                }
            }

            ///---
            ///logging
            ///
            int i = 0;
            foreach (BitArray b in ChunkedArr)
            {
                s += "\n[\n";
                foreach (bool B in b) { s += $"{(B ? "1" : "0")}, "; }
                s += "\n],";
            }

            mainWindow.Output($"LIST LENGTH: {ChunkedArr.Count}\n");
            foreach (BitArray b in ChunkedArr) mainWindow.Output($"ChunkedArr[] Length {b.Length}");
            return s;
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
            if (BitConverter.IsLittleEndian) Array.Reverse(Int64Bytes);
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