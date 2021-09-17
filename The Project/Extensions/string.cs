using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace The_Project.Extensions
{
    public static class StringExtensions
    {
        private static List<IEnumerable<string>> SplitArr(IEnumerable<string> Array)
        {
            /*            if (Array.Count <= 1) return new List<List<string>>() { Array };

                        List<List<string>> NewArr = new() { Array.GetRange(0, (int)Math.Floor((double)Array.Count / 2)), Array.GetRange((int)Math.Floor((double)Array.Count / 2), (int)Math.Ceiling((decimal)Array.Count / 2)) };
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

            if (Array.Count() <= 1) return new List<IEnumerable<string>>() { Array };
            List<IEnumerable<string>> NewArr = new();
            for (int i = 0; i <= Array.Count() - 1; i += 2)
            {
                if (Array.Count() - 1 < i + 1)
                {
                    NewArr.Add(new List<string>() { Array.ElementAt(i) });
                }
                else
                {
                    NewArr.Add(new List<string>() { Array.ElementAt(i), Array.ElementAt(i + 1) });
                }

            }
            return NewArr;
        }

        private static string MergeString(List<IEnumerable<string>> Arr, char Separator)
        {
            Debug.WriteLine(Arr.Count());
            IEnumerable<IEnumerable<string>> Strings = Arr.AsParallel().AsOrdered().Select(x => new List<string>() { string.Join(Separator, x) });
            if (Strings.Count() == 1)
            {
                if (Strings.ElementAt(0).Count() > 1)
                {
                    return Strings.ElementAt(0).ElementAt(0) + '-' + Strings.ElementAt(0).ElementAt(1);
                }
                else
                {
                    return Strings.ElementAt(0).ElementAt(0);
                }
            }
            List<IEnumerable<string>> NewStrings = new();
            for (int i = 0; i <= Strings.Count() - 1; i += 2)
            {
                if (Strings.Count() - 1 < i + 1)
                {
                    NewStrings.Add(Strings.ElementAt(0));
                }
                else
                {
                    NewStrings.Add(new List<string> { Strings.ElementAt(i).ElementAt(0), Strings.ElementAt(i + 1).ElementAt(0) });
                }
            }
            return MergeString(NewStrings, Separator);
        }

        public static string ParallelJoin(this IEnumerable<string> Arr, char Separator)
        {
            DateTime startSplit = DateTime.Now;
            List<IEnumerable<string>> ArrSplit = SplitArr(Arr);
            DateTime endSplit = DateTime.Now;
            Debug.WriteLine($"SPLIT TIME -> {(endSplit - startSplit).TotalMilliseconds}ms");

            Debug.WriteLine(ArrSplit.Count);
            DateTime startMerge = DateTime.Now;
            string Merged = MergeString(ArrSplit, Separator);
            DateTime endMerge = DateTime.Now;
            Debug.WriteLine($"MERGE TIME -> {(endMerge - startMerge).TotalMilliseconds}ms");
            return Merged;
        }
    }
}