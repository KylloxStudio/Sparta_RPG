using System;
using System.Numerics;

public static class BigIntegerExtensions
{
    private static string[] suffixes = { "", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t" };

    public static string Format(this BigInteger number)
    {
        if (number < 1000000)
        {
            return number.ToString("N0");
        }

        int suffixIndex = 0;
        decimal value = (decimal)number;

        while (value >= 1000m && suffixIndex < suffixes.Length - 1)
        {
            value /= 1000m;
            suffixIndex++;
        }

        decimal floored = Math.Floor(value * 100m) / 100m;
        return $"{floored:F2}{suffixes[suffixIndex]}";
    }
}