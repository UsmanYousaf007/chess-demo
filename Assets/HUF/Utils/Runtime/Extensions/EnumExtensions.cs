using System;
using JetBrains.Annotations;

namespace HUF.Utils.Runtime.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns whether an enum value contains a given flag.
        /// </summary>
        /// <param name="enumValue">An enum value.</param>
        /// <param name="flag">A flag.</param>
        [PublicAPI]
        public static bool EnumHasFlag(this Enum enumValue, Enum flag)
        {
            var int64Enum = Convert.ToInt64((object) enumValue);
            var int64Flag = Convert.ToInt64((object) flag);
            return (int64Enum & int64Flag) == int64Flag;
        }
    }
}