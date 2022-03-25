using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace The_Project.Extensions
{
    public static class StringExtensions
    {
        private static List<IList<string>> SplitArr(IList<string> array)
        {
            /*            if (array.Count <= 1) return new List<List<string>>() { array };

                        List<List<string>> NewArr = new() { array.GetRange(0, (int)Math.Floor((double)array.Count / 2)), array.GetRange((int)Math.Floor((double)array.Count / 2), (int)Math.Ceiling((decimal)array.Count / 2)) };
                        if (!NewArr.All(x => x.Count == 2 || x.Count == 1))
                        {
                            List<List<string>> SplitArray = new();
                            foreach (List<string> SubArr in NewArr)
                            {
                                if (SubArr.Count == 2 || SubArr.Count == 1)
                                {
                                    SplitArray.Add(SubArr);
                                    continue;
                                }
                                List<List<string>> SplittedArray = SplitArr(SubArr);
                                SplitArray.AddRange(SplittedArray);
                            }
                            NewArr = SplitArray;
                        }
                        return NewArr;*/

            if (array.Count <= 1)
            {
                return new List<IList<string>> { array };
            }

            List<IList<string>> newArr = new();
            for (int i = 0; i <= array.Count - 1; i += 2)
            {
                newArr.Add(array.Count - 1 < i + 1
                    ? new List<string> { array.ElementAt(i) }
                    : new List<string> { array.ElementAt(i), array.ElementAt(i + 1) });
            }

            return newArr;
        }

        private static string MergeString(IEnumerable<IList<string>> arr, char separator)
        {
            List<IList<string>> listedArr = arr.ToList();
            ParallelQuery<List<string>> strings = listedArr.AsParallel().AsOrdered()
                .Select(x => new List<string> { string.Join(separator, x) });

            int stringsCount = strings.Count();
            IList<string> stringElement0 = strings.ElementAt(0);

            if (stringsCount == 1)
            {
                return stringElement0.Count > 1
                    ? stringElement0.ElementAt(0) + '-' + stringElement0.ElementAt(1)
                    : stringElement0.ElementAt(0);
            }

            List<IList<string>> newStrings = new();
            for (int i = 0; i <= stringsCount - 1; i += 2)
            {
                newStrings.Add(stringsCount - 1 < i + 1
                    ? stringElement0
                    : new List<string> { strings.ElementAt(i).ElementAt(0), strings.ElementAt(i + 1).ElementAt(0) });
            }

            return MergeString(newStrings, separator);
        }

        public static string ParallelJoin(this IList<string> arr, char separator)
        {
            List<IList<string>> arrSplit = SplitArr(arr);
            string mergeString = MergeString(arrSplit, separator);
            return mergeString;
        }
    }
}