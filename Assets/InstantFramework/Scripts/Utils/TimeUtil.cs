/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-10 16:49:45 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;

namespace TurboLabz.TLUtils
{
    public class TimeUtil
    {
        private static readonly DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        public static long unixTimestampMilliseconds
        {
            get
            {
                return (long)((DateTime.UtcNow - UNIX_EPOCH).TotalMilliseconds);
            }
        }

        public static DateTime ToDateTime(double unixTimeStampMilliseconds)
        {
            return UNIX_EPOCH.AddMilliseconds(unixTimeStampMilliseconds);
        }

        public static long ToUnixTimestamp(DateTime dateTime)
        {
            return (long)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
