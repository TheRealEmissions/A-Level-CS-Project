using System.Numerics;

namespace The_Project.Extensions
{
    public static class BigIntegerExtensions
    {
        public static BigInteger GetCoprime(this BigInteger x, BigInteger y)
        {
            while (x < y)
            {
                if (x.GreatestCommonDivider(y) == 1)
                {
                    break;
                }

                x++;
            }
            return x;
        }

        public static BigInteger GreatestCommonDivider(this BigInteger a, BigInteger b)
        {
            BigInteger temp = a % b;
            return temp == 0 ? b : b.GreatestCommonDivider(temp);
        }

        public static bool IsProbablyPrime(this BigInteger n)
        {
            if (n <= 1 || n % 2 == 0)
            {
                return false;
            }

            if (n == 3)
            {
                return true;
            }

            BigInteger a = new BigInteger(2).GetCoprime(n);
            BigInteger result = BigInteger.ModPow(a, n - 1, n);
            return result == 1;
        }
    }
}