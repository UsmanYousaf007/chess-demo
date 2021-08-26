using System;
using HUF.Purchases.Runtime.API;
using UnityEngine;

namespace HUF.Purchases.Runtime.Implementation.Data
{
    [Serializable]
    public class SubscriptionResponse
    {
        public bool providerOverride = false;
        public bool valid = false;

        //needed
        public bool isAutoRenewing = false;

        public bool isFreeTrial = false;

        //maybe needed
        public bool isSubscribed = true;

        [SerializeField] string cancellationDate = string.Empty;
        [SerializeField] string currentDate = string.Empty;
        [SerializeField] string expiresDate = string.Empty;

        public DateTime CancellationDate => ParseDate( cancellationDate );
        public DateTime CurrentDate => ParseDate( currentDate );
        public DateTime ExpiresDate => ParseDate( expiresDate );
        public bool IsExpired => CurrentDate >= ExpiresDate;

        DateTime ParseDate( string dateString )
        {
            if ( dateString == "" )
            {
                return PurchasesDateTimeUtils.CurrentUTCDateTime;
            }

            return DateTime.Parse( dateString, null, System.Globalization.DateTimeStyles.RoundtripKind );
        }
    }
}