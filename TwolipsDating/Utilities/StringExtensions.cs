using System;
using System.Text.RegularExpressions;

namespace TwolipsDating.Utilities
{
    public static class StringExtensions
    {
        internal static string ToSEOFriendlyString(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return String.Empty;
            }

            return Regex.Replace(value.ToLower().Replace(@"'", String.Empty), @"[^\w]+", "-");
        }
    }
}