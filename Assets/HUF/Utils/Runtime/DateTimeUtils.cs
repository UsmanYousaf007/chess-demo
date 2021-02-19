using System;
using JetBrains.Annotations;

namespace HUF.Utils.Runtime
{
    public static class DateTimeUtils
    {
        /// <summary>
        /// Marks a period of time from which UTC timestamp is counted
        /// </summary>
        [PublicAPI]
        public static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts a timestamp to a DateTime.
        /// </summary>
        /// <param name="timestamp">A timestamp in seconds.</param>
        /// <returns>A DateTime.</returns>
        [PublicAPI]
        public static DateTime FromTimestamp(long timestamp)
        {
            return epoch.AddSeconds(timestamp);
        }

        /// <summary>
        /// Converts a DateTime to a timestamp.
        /// </summary>
        /// <param name="dateTime">A DateTime.</param>
        /// <returns>A timestamp.</returns>
        [PublicAPI]
        public static long ToTimestamp(this DateTime dateTime)
        {
            return dateTime.TotalSeconds();
        }

        static long TotalSeconds(this DateTime dateTime)
        {
            return (long) (dateTime - epoch).TotalSeconds;
        }
    }
}