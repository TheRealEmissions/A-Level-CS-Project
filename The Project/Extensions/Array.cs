using System.Collections.Generic;
using System.Linq;

namespace The_Project.Extensions
{
    internal static class Array<T>
    {
        internal static List<IList<T>> SplitArr(IList<T> arr)
        {
            if (arr.Count <= 1)
            {
                return new List<IList<T>> { arr };
            }

            List<IList<T>> newArr = new();
            for (int i = 0; i <= arr.Count - 1; i += 2)
            {
                newArr.Add(i >= arr.Count - 1
                    ? new List<T> { arr.ElementAt(i) }
                    : new List<T> { arr.ElementAt(i), arr.ElementAt(i + 1) });
            }

            return newArr;
        }
    }
}