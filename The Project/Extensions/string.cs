using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace The_Project.Extensions
{
    public static class StringExtensions
    {
        private static List<List<string>> SplitArr(List<string> Array)
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

            if (Array.Count <= 1) return new List<List<string>>() { Array };
            List<List<string>> NewArr = new();
            for (int i = 0; i <= Array.Count - 1; i += 2)
            {
                if (Array.Count - 1 < i + 1)
                {
                    NewArr.Add(new List<string>() { Array[i] });
                } else
                {
                    NewArr.Add(new List<string>() { Array[i], Array[i + 1] });
                }

            }
            return NewArr;
        }

        private static string MergeString(List<List<string>> Arr, char Separator)
        {
            Debug.WriteLine(Arr.Count);
            List<List<string>> Strings = Arr.AsParallel().AsOrdered().WithDegreeOfParallelism(Environment.ProcessorCount).Select(x => new List<string>() { string.Join(Separator, x) }).ToList();
            if (Strings.Count == 1)
            {
                if (Strings[0].Count > 1)
                {
                    return string.Join(Separator, Strings[0]);
                } else
                {
                    return Strings[0][0];
                }
            }
            List<List<string>> NewStrings = new();
            for (int i = 0; i <= Strings.Count - 1; i += 2)
            {
                if (Strings.Count - 1 < i + 1)
                {
                    NewStrings.Add(Strings[i]);
                }
                else
                {
                    NewStrings.Add(new List<string> { Strings[i][0], Strings[i + 1][0] });
                }
            }
            return MergeString(NewStrings, Separator);
        }

        public static string ParallelJoin(this IEnumerable<string> Arr, char Separator)
        {
            List<List<string>> ArrSplit = SplitArr(Arr.ToList());
            Debug.WriteLine(ArrSplit.Count);
            string Merged = MergeString(ArrSplit, Separator);
            return Merged;
        }
    }
}