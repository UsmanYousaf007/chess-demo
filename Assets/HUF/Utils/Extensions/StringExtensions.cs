using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HUF.Utils.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNotEmpty(this string text)
        {
            return !string.IsNullOrEmpty(text);
        }

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }
        
        public static string CamelCaseToUnderscores(this string text)
        {
            return Regex.Replace(text, "(?<=.)([A-Z])", "_$0", RegexOptions.Compiled).ToLower();
        }
             
        public static IEnumerable<string> SplitByDot(this string key)
        {
            return key.Split('.');
        }
    }
}