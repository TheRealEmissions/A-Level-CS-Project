using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Utils
{
    public class Bitwise
    {
        public static BitArray RightRotate(BitArray bitArray, int bits)
        {
            BitArray Temp = new(bitArray);
            bitArray.RightShift(bits);
            for (int i = 0; i < bits; i++)
            {
                bitArray[i] = Temp[bitArray.Length - 1 - bits + i];
            }
            return bitArray;
        }

        public static BitArray LeftRotate(BitArray bitArray, int bits)
        {
            BitArray Temp = new(bitArray);
            bitArray.LeftShift(bits);
            for (int i = 0; i < bits; i++)
            {
                bitArray[bitArray.Length - 1 - bits + i] = Temp[i];
            }
            return bitArray;
        }
    }
}
