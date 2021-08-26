using System;
#if HUF_TIMESERVER
using HUF.TimeServer.Runtime.API;
#endif
using HUF.Utils.Runtime;

namespace HUF.Purchases.Runtime.Implementation
{
    [Serializable]
    public static class PurchasesDateTimeUtils
    {
        /// <summary>
        /// Returns the current UTC date time.
        /// </summary>
        internal static DateTime CurrentUTCDateTime
        {
            get
            {
#if HUF_TIMESERVER
                if ( HTimeServer.GetTime() > 0 )
                    return DateTimeUtils.FromTimestamp( HTimeServer.GetTime() );
#endif
                return DateTime.UtcNow;
            }
        }
    }
}