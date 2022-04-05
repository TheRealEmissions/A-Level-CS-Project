using System;
using System.Collections;

namespace The_Project.Extensions
{
    internal static class BitArrayExtension
    {
        internal static BitArray SafeRightShift(this BitArray bitArray, int bits)
        {
            BitArray tempArr = new(bitArray);
            tempArr.RightShift(bits);
            return tempArr;
        }

        private static BitArray SafeLeftShift(this BitArray bitArray, int bits)
        {
            BitArray tempArr = new(bitArray);
            tempArr.LeftShift(bits);
            return tempArr;
        }

        public static BitArray SafeRightRotate(this BitArray bitArray, int bits)
        {
            BitArray tempArr = new(bitArray);
            BitArray arr = bitArray.SafeRightShift(bits);
            for (int i = 0; i < bits; i++)
            {
                arr[i] = tempArr[arr.Length - 1 - bits + i];
            }

            return arr;
        }

        public static BitArray SafeLeftRotate(this BitArray bitArray, int bits)
        {
            BitArray tempArr = new(bitArray);
            BitArray arr = bitArray.SafeLeftShift(bits);
            for (int i = 0; i < bits; i++)
            {
                arr[arr.Length - 1 - bits + i] = tempArr[i];
            }

            return arr;
        }

        public static BitArray SafeXor(this BitArray bitArray1, BitArray bitArray2)
        {
            BitArray tempArr = new(bitArray1);
            BitArray arr = tempArr.Xor(bitArray2);
            return arr;
        }

        public static uint ToUInt32(this BitArray bitArray)
        {
            uint[] arr = new uint[1];
            bitArray.CopyTo(arr, 0);
            return arr[0];
        }

        public static BitArray Add(this BitArray bitArray1, BitArray bitArray2)
        {
            ulong i = bitArray1.ToUInt32();
            ulong j = bitArray2.ToUInt32();
            uint k = (uint) ((i + j) % 4294967296);
            byte[] bytes = BitConverter.GetBytes(k);
            BitArray array = new(bytes);
            return array;
        }

        public static BitArray SafeAnd(this BitArray bitArray1, BitArray bitArray2)
        {
            BitArray tempArr = new(bitArray1);
            BitArray arr = tempArr.And(bitArray2);
            return arr;
        }

        public static BitArray SafeNot(this BitArray bitArray1)
        {
            BitArray tempArr = new(bitArray1);
            BitArray arr = tempArr.Not();
            return arr;
        }

        public static byte[] ToByteArray(this BitArray bitArray)
        {
            byte[] bytes = new byte[bitArray.Length / 8];
            bitArray.CopyTo(bytes, 0);
            return bytes;
        }
    }
}