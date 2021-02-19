using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace HUF.Utils.Runtime.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns whether a given string is not empty.
        /// </summary>
        /// <param name="text">A string to be checked.</param>
        [PublicAPI]
        public static bool IsNotEmpty( this string text )
        {
            return !string.IsNullOrEmpty( text );
        }

        /// <summary>
        /// Returns whether a given string is not a null nor empty.
        /// </summary>
        /// <param name="text">A string to be checked.</param>
        [PublicAPI]
        public static bool IsNullOrEmpty( this string text )
        {
            return string.IsNullOrEmpty( text );
        }

        /// <summary>
        /// Converts camel case string to underscore string.
        /// </summary>
        /// <param name="text">A string to be converted.</param>
        [PublicAPI]
        public static string CamelCaseToUnderscores( this string text )
        {
            return Regex.Replace( text, "(?<=.)([A-Z])", "_$0", RegexOptions.Compiled ).ToLower();
        }

        /// <summary>
        /// Splits a string using dot as the separator.
        /// </summary>
        /// <param name="text">A string to be split.</param>
        /// <returns>A split string.</returns>
        [PublicAPI]
        public static IEnumerable<string> SplitByDot( this string text )
        {
            return text.Split( '.' );
        }
    }
}