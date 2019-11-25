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
            return (long)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
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
                return Mathf.Max(1, Mathf.CeilToInt((float)elapsedTime.TotalDays)) + " " + localizeDays;
            }
        }

        public static string FormatPlayerClock(TimeSpan timer)
        {
            // TODO: localize clock

            long seconds = timer.Seconds;
            long minutes = timer.Minutes;
            long hours = timer.Hours;

            if (timer.Days > 0)
            {
                hours += timer.Days * 24;
            }

            if (timer.TotalHours >= 1)
            {
                return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
            }

            // else show 00:00 format
            // This code is similar to rounding the seconds to a ceiling
            if (timer.TotalMilliseconds > 0)
            {
                timer = TimeSpan.FromMilliseconds(timer.TotalMilliseconds + 999);
            }

            return string.Format("{0:00}:{1:00}", Mathf.FloorToInt((float)timer.TotalMinutes), timer.Seconds);
        }

        public static string FormatStripClock(TimeSpan timer)
        {
            if (Mathf.FloorToInt((float)timer.TotalHours) == 48)
            {
                return "48h";
            }
            else if (timer.TotalHours >= 1)
            {
                return Mathf.FloorToInt((float)timer.TotalHours) + "h+";
            }
            else 
            {
                return "< 1h";
            }

        }

        public static string DateTimeToRelativeTime(DateTime dateTime)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - dateTime.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "a second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return ts.Hours + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return ts.Days + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "a month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "an year ago" : years + " years ago";
            }
        }
    }
}
