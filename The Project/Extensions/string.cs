using System.Collections.Generic;
using System.Linq;

namespace The_Project.Extensions
{
    public static class StringExtensions
    {
        private static List<IEnumerable<string>> SplitArr(IEnumerable<string> array)
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

            if (array.Count() <= 1)
            {
                return new List<IEnumerable<string>>() {array};
            }

            List<IEnumerable<string>> newArr = new();
            for (int i = 0; i <= array.Count() - 1; i += 2)
            {
                newArr.Add(array.Count() - 1 < i + 1
                    ? new List<string> {array.ElementAt(i)}
                    : new List<string> {array.ElementAt(i), array.ElementAt(i + 1)});
            }

            return newArr;
        }

        private static string MergeString(List<IEnumerable<string>> arr, char separator)
        {
            IEnumerable<IEnumerable<string>> strings = arr.AsParallel().AsOrdered()
                .Select(x => new List<string>() {string.Join(separator, x)});
            if (strings.Count() == 1)
            {
                return strings.ElementAt(0).Count() > 1
                    ? strings.ElementAt(0).ElementAt(0) + '-' + strings.ElementAt(0).ElementAt(1)
                    : strings.ElementAt(0).ElementAt(0);
            }

            List<IEnumerable<string>> newStrings = new();
            for (int i = 0; i <= strings.Count() - 1; i += 2)
            {
                newStrings.Add(strings.Count() - 1 < i + 1
                    ? strings.ElementAt(0)
                    : new List<string> {strings.ElementAt(i).ElementAt(0), strings.ElementAt(i + 1).ElementAt(0)});
            }

            return MergeString(newStrings, separator);
        }

        public static string ParallelJoin(this IEnumerable<string> arr, char separator)
        {
            List<IEnumerable<string>> arrSplit = SplitArr(arr);
            string mergeString = MergeString(arrSplit, separator);
            return mergeString;
        }
    }
}