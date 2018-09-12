/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using UnityEngine;

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

        public static string TimeToExpireString(long msUTC, int expireDays, 
            string localizeMins = "", string localizeHours = "", string localizeDays = "")
        {
            DateTime expireDate = TurboLabz.TLUtils.TimeUtil.ToDateTime(msUTC);
            expireDate = expireDate.AddDays(expireDays);

            TimeSpan elapsedTime;
            if (DateTime.Compare(expireDate, DateTime.UtcNow) > 0)
            {
                elapsedTime = expireDate.Subtract(DateTime.UtcNow);
            }
            else
            {
                return null;
            }

            if (elapsedTime.TotalHours < 1)
            {
                return Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalMinutes)) + " " + localizeMins;
            }
            else if (elapsedTime.TotalDays < 1)
            {
                return  Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalHours)) + " " + localizeHours;
            }
            else
            {
                return Mathf.Max(1, Mathf.FloorToInt((float)elapsedTime.TotalDays)) + " " + localizeDays;
            }
        }
    }
}
