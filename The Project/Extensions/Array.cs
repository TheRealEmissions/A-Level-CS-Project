using System.Collections.Generic;
using System.Linq;

namespace The_Project.Extensions
{
    public static class Array<T>
    {
        public static List<IEnumerable<T>> SplitArr(IEnumerable<T> Arr)
        {
            if (Arr.Count() <= 1)
            {
                return new List<IEnumerable<T>>() { Arr };
            }

            List<IEnumerable<T>> NewArr = new();
            for (int i = 0; i <= Arr.Count() - 1; i += 2)
            {
                if (Arr.Count() - 1 <= i)
                {
                    NewArr.Add(new List<T>() { Arr.ElementAt(i) });
                }
                else
                {
                    NewArr.Add(new List<T>() { Arr.ElementAt(i), Arr.ElementAt(i + 1) });
                }
            }
            return NewArr;
        }
    }
}