using System;

namespace HUF.Utils.Runtime
{
    public static class DateTimeUtils
    {
        static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public static DateTime FromTimestamp(long timestamp)
        {
            return epoch.AddSeconds(timestamp);
        }
        
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