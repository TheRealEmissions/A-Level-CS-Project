using System;
using System.Collections;
using System.Collections.Generic;
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

        private uint h0 = 0x6a09e667;
        private uint h1 = 0xbb67ae85;
        private uint h2 = 0x3c6ef372;
        private uint h3 = 0xa54ff53a;
        private uint h4 = 0x510e527f;
        private uint h5 = 0x9b05688c;
        private uint h6 = 0x1f83d9ab;
        private uint h7 = 0x5be0cd19;

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
            // SPLITS INTO 32 bit chunks (16 originally, then adds 48 initalised to 0 for 64 bit)
            // ***

            foreach (BitArray chunk in ChunkedArr)
            {
                List<BitArray> chunk32bit = SplitIntoChunks(chunk, 32); // 16 entries
                for (int j = 0; j < 48; j++)
                {
                    chunk32bit.Add(new BitArray(32)); // add 48 more to bring to 64 (256 bytes)
                }

                for (int j = 16; j < 64; j++)
                {
                    BitArray s0 = chunk32bit[j - 15].SafeRightRotate(7).SafeXor(chunk32bit[j - 15].SafeRightRotate(18)).SafeXor(chunk32bit[j - 15].SafeRightShift(3));
                    BitArray s1 = chunk32bit[j - 2].SafeRightRotate(17).SafeXor(chunk32bit[j - 2].SafeRightRotate(19)).SafeXor(chunk32bit[j - 2].SafeRightShift(10));
                    chunk32bit[j] = chunk32bit[j - 16].Add(s0).Add(chunk32bit[j - 7]).Add(s1);
                }

                BitArray a = new BitArray(BitConverter.GetBytes(h0));
                BitArray b = new BitArray(BitConverter.GetBytes(h1));
                BitArray c = new BitArray(BitConverter.GetBytes(h2));
                BitArray d = new BitArray(BitConverter.GetBytes(h3));
                BitArray e = new BitArray(BitConverter.GetBytes(h4));
                BitArray f = new BitArray(BitConverter.GetBytes(h5));
                BitArray g = new BitArray(BitConverter.GetBytes(h6));
                BitArray h = new BitArray(BitConverter.GetBytes(h7));

                for (int j = 0; j < 64; j++)
                {
                    BitArray S1 = e.SafeRightRotate(6).SafeXor(e.SafeRightRotate(11)).SafeXor(e.SafeRightRotate(25));
                    BitArray ch = e.SafeAnd(f).SafeXor(e.SafeNot().SafeAnd(g));
                    BitArray temp1 = h.Add(S1).Add(ch).Add(new BitArray(BitConverter.GetBytes(k[j]))).Add(chunk32bit[j]);
                    BitArray S0 = a.SafeRightRotate(2).SafeXor(a.SafeRightRotate(13)).SafeXor(a.SafeRightRotate(22));
                    BitArray maj = a.SafeAnd(b).SafeXor(a.SafeAnd(c)).SafeXor(b.SafeAnd(c));
                    BitArray temp2 = S0.Add(maj);

                    h = g;
                    g = f;
                    f = e;
                    e = d.Add(temp1);
                    d = c;
                    c = b;
                    b = a;
                    a = temp1.Add(temp2);
                }

                BitArray temph0 = new BitArray(BitConverter.GetBytes(h0)).Add(a);
                BitArray temph1 = new BitArray(BitConverter.GetBytes(h1)).Add(b);
                BitArray temph2 = new BitArray(BitConverter.GetBytes(h2)).Add(c);
                BitArray temph3 = new BitArray(BitConverter.GetBytes(h3)).Add(d);
                BitArray temph4 = new BitArray(BitConverter.GetBytes(h4)).Add(e);
                BitArray temph5 = new BitArray(BitConverter.GetBytes(h5)).Add(f);
                BitArray temph6 = new BitArray(BitConverter.GetBytes(h6)).Add(g);
                BitArray temph7 = new BitArray(BitConverter.GetBytes(h7)).Add(h);

                h0 = temph0.ToUInt32();
                h1 = temph1.ToUInt32();
                h2 = temph2.ToUInt32();
                h3 = temph3.ToUInt32();
                h4 = temph4.ToUInt32();
                h5 = temph5.ToUInt32();
                h6 = temph6.ToUInt32();
                h7 = temph7.ToUInt32();
            }

            return h0.ToString("X").PadLeft(8, '0') + h1.ToString("X").PadLeft(8, '0') + h2.ToString("X").PadLeft(8, '0') + h3.ToString("X").PadLeft(8, '0') + h4.ToString("X").PadLeft(8, '0') + h5.ToString("X").PadLeft(8, '0') + h6.ToString("X").PadLeft(8, '0') + h7.ToString("X").PadLeft(8, '0');
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