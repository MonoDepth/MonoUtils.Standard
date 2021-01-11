using System;
using System.Net.Http;
using System.Collections.Generic;

namespace MonoUtilities.Conversions
{
    public static class Conversions
    {
        public static DateTime FromUnixTime(this long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }

        public static bool IsInt(this string s)
        {
            return int.TryParse(s, out int _);
        }
        public static bool IsInt(this char c)
        {
            string s = c.ToString();
            return int.TryParse(s, out int _);
        }

        public static bool IsGreaterThan<T>(this T value, T other)
        {
            return Comparer<T>.Default.Compare(value, other) > 0;
        }

        public static bool IsLessThan<T>(this T value, T other)
        {
            return Comparer<T>.Default.Compare(value, other) < 0;
        }


        public static FormUrlEncodedContent KeyPairsToHttpContent(this List<KeyValuePair<string, string>> keyPair)
        {
            return new FormUrlEncodedContent(keyPair);
        }
        public static string GetSubstr(this string str, string find)
        {
            return str.Substring(str.IndexOf(find));
        }
    }
}