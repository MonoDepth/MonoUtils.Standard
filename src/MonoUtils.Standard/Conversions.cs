using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

        public static string[] SplitLettersAndNumbers(this string input)
        {
            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
            Match result = re.Match(input);

            string alphaPart = result.Groups[1].Value;
            string numberPart = result.Groups[2].Value;

            return new[] { alphaPart, numberPart };
        }

        public static string IncrementString(this string input, int iterations = 1)
        {
            if (iterations == 0)
            {
                return input.ToUpper();
            }

            string rtn = "A";
            if (!string.IsNullOrWhiteSpace(input))
            {
                bool prependNew = false;
                var sb = new StringBuilder(input.ToUpper());
                for (int it = 0; it < iterations; it++)
                {
                    for (int i = (sb.Length - 1); i >= 0; i--)
                    {
                        if (i == sb.Length - 1)
                        {
                            var nextChar = Convert.ToUInt16(sb[i]) + 1;
                            if (nextChar > 90)
                            {
                                sb[i] = 'A';
                                if ((i - 1) >= 0)
                                {
                                    sb[i - 1] = (char)(Convert.ToUInt16(sb[i - 1]) + 1);
                                }
                                else
                                {
                                    prependNew = true;
                                }
                            }
                            else
                            {
                                sb[i] = (char)(nextChar);
                                break;
                            }
                        }
                        else
                        {
                            if (Convert.ToUInt16(sb[i]) > 90)
                            {
                                sb[i] = 'A';
                                if ((i - 1) >= 0)
                                {
                                    sb[i - 1] = (char)(Convert.ToUInt16(sb[i - 1]) + 1);
                                }
                                else
                                {
                                    prependNew = true;
                                }
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                }
                rtn = sb.ToString();
                if (prependNew)
                {
                    rtn = "A" + rtn;
                }
            }

            return rtn.ToUpper();
        }
    }
}