using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace HUF.Utils.Runtime.Extensions
{
    public static class StringExtensions
    {
        static readonly StringComparison caseInsensitiveStringComparison = StringComparison.CurrentCultureIgnoreCase;

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

        /// <summary>
        /// Checks if a string contains a value when case is ignored.
        /// </summary>
        /// <param name="text">A string to be checked.</param>
        /// <param name="searchedValue">A searched string.</param>
        /// <returns>Whether a string contains a searched value.</returns>
        [PublicAPI]
        public static bool ContainsCaseInsensitive( this string text, string searchedValue )
        {
            return text.IndexOf( searchedValue, caseInsensitiveStringComparison ) >= 0;
        }

        /// <summary>
        /// Gets the substring of a string without raising exceptions.
        /// </summary>
        /// <param name="text">A string to be checked.</param>
        [PublicAPI]
        public static string SafeSubstring( this string text, int start, int length )
        {
            return text.Length <= start ? ""
                : text.Length - start <= length ? text.Substring( start )
                : text.Substring( start, length );
        }

        /// <summary>
        /// Replaces first instance of the searched value with the new value;
        /// </summary>
        /// <param name="text">A string to be checked.</param>
        /// <param name="searchedValue">A searched string.</param>
        /// <param name="newValue">A new value that will replace the searched string.</param>
        /// <returns>Returns text after replacing the searched value.</returns>
        [PublicAPI]
        public static string ReplaceFirst( this string text, string searchedValue, string newValue )
        {
            int pos = text.IndexOf( searchedValue );

            if ( pos < 0 )
            {
                return text;
            }

            return text.Substring( 0, pos ) + newValue + text.Substring( pos + searchedValue.Length );
        }
    }
}