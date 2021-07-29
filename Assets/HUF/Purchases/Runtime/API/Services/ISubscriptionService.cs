#if UNITY_PURCHASING
using System;
using HUF.Purchases.Runtime.API.Data;
using UnityEngine.Purchasing;

namespace HUF.Purchases.Runtime.API.Services
{
    public interface ISubscriptionService
    {
        event Action<ISubscriptionPurchaseData> OnSubscriptionPurchase;
        event Action<IProductInfo> OnSubscriptionExpired;

        bool IsSubscriptionActive( string id );
        void UpdateSubscriptions( Product[] products );
        SubscriptionStatus GetStatus( string id );
        bool IsInTrialMode( string id );
        DateTime GetExpirationDate( string id );
    }
}
#endif
