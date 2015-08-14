using System;
using System.Linq;

namespace ken.Core.Types
{
    public static class StringExtensions
    {
        public static bool IsInteger(this string str)
        {
            if (String.IsNullOrWhiteSpace(str)) return false;
            return str.All(c => c >= '0' && c <= '9');
        }

        public static string Left(this string value, int length)
        {
            return value.Substring(0, length);
        }

        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }
    }
}
