using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace The_Project.Encryption
{
    public class Hashing
    {
        public Hashing() { }

        public static string Hash(string str)
        {
            char[] chars = str.ToCharArray();
            List<int> numbers = new();

            foreach (char c in chars) numbers.Add((int)c % 101);

            List<int> newNumbers = new();

            for (int i = 0; i < numbers.Count / 2; i += 2)
            {
                int n = numbers.Count - 1 <= i + 1 ? numbers[i] + numbers[i + 1] : numbers[i];
                newNumbers.Add(n % 11);
            }

            string s = "";
            foreach (int n in newNumbers) s += n.ToString();
            return s;
        }
    }
}
