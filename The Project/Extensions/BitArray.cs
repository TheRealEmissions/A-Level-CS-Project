using System;
using System.Collections;

namespace The_Project.Extensions
{
    public static class BitArrayExtension
    {
        public static BitArray SafeRightShift(this BitArray bitArray, int bits)
        {
            BitArray tempArr = new BitArray(bitArray);
            tempArr.RightShift(bits);
            return tempArr;
        }

        public static BitArray SafeLeftShift(this BitArray bitArray, int bits)
        {
            BitArray tempArr = new BitArray(bitArray);
            tempArr.LeftShift(bits);
            return tempArr;
        }

        public static BitArray SafeRightRotate(this BitArray bitArray, int bits)
        {
            BitArray TempArr = new(bitArray);
            BitArray Arr = bitArray.SafeRightShift(bits);
            for (int i = 0; i < bits; i++)
            {
                Arr[i] = TempArr[Arr.Length - 1 - bits + i];
            }
            return Arr;
        }

        public static BitArray SafeLeftRotate(this BitArray bitArray, int bits)
        {
            BitArray TempArr = new(bitArray);
            BitArray Arr = bitArray.SafeLeftShift(bits);
            for (int i = 0; i < bits; i++)
            {
                Arr[Arr.Length - 1 - bits + i] = TempArr[i];
            }
            return Arr;
        }

        public static BitArray SafeXor(this BitArray bitArray1, BitArray bitArray2)
        {
            BitArray TempArr = new(bitArray1);
            BitArray Arr = TempArr.Xor(bitArray2);
            return Arr;
        }

        public static uint ToUInt32(this BitArray bitArray)
        {
            uint[] Arr = new uint[1];
            bitArray.CopyTo(Arr, 0);
            return Arr[0];
        }

        public static BitArray Add(this BitArray bitArray1, BitArray bitArray2)
        {
            ulong i = bitArray1.ToUInt32();
            ulong j = bitArray2.ToUInt32();
            uint k = (uint)((i + j) % 4294967296);
            byte[] bytes = BitConverter.GetBytes(k);
            BitArray Arr = new(bytes);
            return Arr;
        }

        public static BitArray SafeAnd(this BitArray bitArray1, BitArray bitArray2)
        {
            BitArray TempArr = new BitArray(bitArray1);
            BitArray Arr = TempArr.And(bitArray2);
            return Arr;
        }

        public static BitArray SafeNot(this BitArray bitArray1)
        {
            BitArray TempArr = new BitArray(bitArray1);
            BitArray Arr = TempArr.Not();
            return Arr;
        }

        public static int Square(this int value)
        {
            return value * value;
        }
    }
}