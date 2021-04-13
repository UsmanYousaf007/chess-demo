#if UNITY_PURCHASING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HUF.Purchases.Runtime.API;
using HUF.Purchases.Runtime.API.Data;
using HUF.Purchases.Runtime.API.Services;
using HUF.Purchases.Runtime.Implementation.Data;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime._3rdParty.Blowfish;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using HUF.Utils.Runtime.PlayerPrefs.SecureTypes;
using UnityEngine;
using UnityEngine.Purchasing;

namespace HUF.Purchases.Runtime.Implementation.Services
{
    public class SubscriptionService : ISubscriptionService
    {
#pragma warning disable 0618
        public event Action<ISubscriptionPurchaseData> OnSubscriptionPurchase;
        public event Action<IProductInfo> OnSubscriptionExpired;
        const string SUBSCRIPTION_SAVE_PATH = "SubscriptionService.usedSubscriptions.";
        const int DEFAULT_DEBUG_SUBSCRIPTION_DURATION = 5;

        static readonly HLogPrefix logPrefix = new HLogPrefix( HPurchases.logPrefix, nameof(SubscriptionService) );

#if DEBUG
#if UNITY_IOS
        // Values based on: https://www.revenuecat.com/blog/the-ultimate-guide-to-subscription-testing-on-ios
        Dictionary<int,int> DaysToMinutesDebug { get; } = new Dictionary<int, int>
        {
            {3, 2}, {7, 3}, {30, 5}, {28, 5}, {29, 5}, {31, 5}, {30 * 2, 10}, {28 * 2, 10}, {29 * 2, 10}, {31 * 2, 10},
            {30 * 3, 15}, {28 * 3, 15}, {29 * 3, 15}, {31 * 3, 15}, {30 * 6, 30}, {28 * 6, 15}, {29 * 6, 30},
            {31 * 6, 30}, {365, 60}
        };
#else
        // Values based on https://developer.android.com/google/play/billing/test#renewals
        Dictionary<int, int> DaysToMinutesDebug { get; } = new Dictionary<int, int>
        {
            { 7, 5 }, { 30, 5 }, { 28, 5 }, { 29, 5 }, { 31, 5 }, { 365, 30 }, { 6 * 30, 15 }, { 6 * 31, 15 },
            { 6 * 28, 15 }, { 6 * 29, 15 }
        };
#endif
#endif
        readonly BlowFish encryption;
        readonly IEnumerable<IProductInfo> productsInfo;
        readonly Dictionary<string, SecureCustomPP<SubscriptionSaveData>> subscriptionCachedSaves;
        readonly Dictionary<string, SubscriptionInfo> subscriptionsInfo;

        internal SubscriptionService( IEnumerable<IProductInfo> productsInfo,
            BlowFish encryption = null )
        {
            this.productsInfo = productsInfo;
            this.encryption = encryption;
            subscriptionsInfo = new Dictionary<string, SubscriptionInfo>();
            subscriptionCachedSaves = new Dictionary<string, SecureCustomPP<SubscriptionSaveData>>();
        }

        public void UpdateSubscriptions( Product[] products )
        {
            foreach ( var product in products )
            {
                if ( product.definition.type != ProductType.Subscription )
                    continue;

                SubscriptionInfo info = null;

                if ( product.hasReceipt && IsAvailableForSubscriptionManager( product.receipt ) )
                {
                    var manager = new SubscriptionManager( product, null );
                    info = manager.getSubscriptionInfo();
                }

                UpdateSubscriptionInfo( product.definition.id, info );
            }

            if ( Debug.isDebugBuild )
                LogSubscriptions();
        }

        public bool IsSubscriptionActive( string id )
        {
            return id != null && GetStatus( id ) == SubscriptionStatus.Active;
        }

        public SubscriptionStatus GetStatus( string id )
        {
            if ( id == null || subscriptionsInfo == null || !subscriptionsInfo.ContainsKey( id ) )
            {
                HLog.LogWarning( logPrefix, $"Can't find subscriptions with id: {id} {subscriptionsInfo == null}" );
                return SubscriptionStatus.Unknown;
            }

            var status = SubscriptionStatus.Unknown;
            var info = subscriptionsInfo[id];

            if ( info == null && IsSubscriptionSave( id ) )
                status = SubscriptionStatus.Expired;
            else if ( IsExpired( info ) )
                status = SubscriptionStatus.Expired;
            else if ( IsSubscribed( info ) )
                status = SubscriptionStatus.Active;
            return status;
        }

        public bool IsInTrialMode( string id )
        {
            if ( subscriptionsInfo == null || !subscriptionsInfo.ContainsKey( id ) )
            {
                HLog.LogWarning( logPrefix, $"Can't find subscriptions with id: {id}" );
                return false;
            }

            var info = subscriptionsInfo[id];

            return info != null &&
                   info.isFreeTrial() == Result.True &&
                   IsTimeNewer( DateTime.UtcNow, info.getExpireDate() ) == false;
        }

        public DateTime GetExpirationDate( string id )
        {
            if ( subscriptionsInfo == null || !subscriptionsInfo.ContainsKey( id ) )
            {
                HLog.LogWarning( logPrefix, $"Can't find subscriptions with id: {id}" );
                return DateTime.UtcNow;
            }

            var info = subscriptionsInfo[id];

            if ( info != null )
            {
                return info.isExpired() != Result.Unsupported
                    ? info.getExpireDate()
                    : GetUnsupportedSubscriptionExpirationDate( info );
            }

            return DateTime.UtcNow;
        }

        DateTime GetUnsupportedSubscriptionExpirationDate( SubscriptionInfo info )
        {
            var productInfo = productsInfo.FirstOrDefault( q => info.getProductId().EndsWith( q.ProductId ) );

            if ( productInfo != null )
            {
#if DEBUG
                return info.getPurchaseDate()
                    .AddMinutes( DaysToMinutesDebug.TryGetValue( productInfo.SubscriptionPeriod, out int minutes )
                        ? minutes
                        : DEFAULT_DEBUG_SUBSCRIPTION_DURATION );

#else
                return info.getPurchaseDate().AddDays( productInfo.SubscriptionPeriod );
#endif
            }

            return info.getExpireDate();
        }

        bool IsAvailableForSubscriptionManager( string receipt )
        {
            var receiptWrapper = (Dictionary<string, object>)MiniJson.JsonDecode( receipt );

            if ( !receiptWrapper.ContainsKey( "Store" ) || !receiptWrapper.ContainsKey( "Payload" ) )
            {
                HLog.LogError( logPrefix, "The product receipt does not contain enough information" );
                return false;
            }

            var store = (string)receiptWrapper["Store"];
            var payload = (string)receiptWrapper["Payload"];

            if ( payload == null )
                return false;

            switch ( store )
            {
                case GooglePlay.Name:
                {
                    return ProcessGooglePlaySubscription( payload );
                }
                case AppleAppStore.Name:
                case AmazonApps.Name:
                case MacAppStore.Name:
                {
                    return true;
                }
                default:
                {
                    return false;
                }
            }
        }

        bool ProcessGooglePlaySubscription( string payload )
        {
            var payloadWrapper = (Dictionary<string, object>)MiniJson.JsonDecode( payload );

            if ( !payloadWrapper.ContainsKey( "json" ) )
            {
                HLog.LogError( logPrefix, "The product receipt is messed, the 'json' field is missing" );
                return false;
            }

            var originalJsonPayloadWrapper =
                (Dictionary<string, object>)MiniJson.JsonDecode( (string)payloadWrapper["json"] );

            if ( originalJsonPayloadWrapper == null ||
                 !originalJsonPayloadWrapper.ContainsKey( "developerPayload" ) )
            {
                HLog.LogError( logPrefix, "The product receipt is messed, the 'developerPayload' field is missing" );
                return false;
            }

            var developerPayloadWrapper = (Dictionary<string, object>)MiniJson.JsonDecode(
                (string)originalJsonPayloadWrapper["developerPayload"] );

            if ( developerPayloadWrapper == null ||
                 !developerPayloadWrapper.ContainsKey( "is_free_trial" ) ||
                 !developerPayloadWrapper.ContainsKey( "has_introductory_price_trial" ) )
            {
                HLog.LogError( logPrefix,
                    "The product receipt is messed, the product is not purchased using 1.19 or later" );
                return false;
            }

            return true;
        }

        void UpdateSubscriptionInfo( string id, SubscriptionInfo info )
        {
            var productInfo = productsInfo.FirstOrDefault( q => q.ProductId == id );

            if ( productInfo == null )
                return;

            if ( info == null || info.isExpired() == Result.True )
            {
                if ( TryDeleteSave( id ) )
                    OnSubscriptionExpired.Dispatch( productInfo );
                return;
            }

            if ( subscriptionsInfo.ContainsKey( id ) )
                subscriptionsInfo[id] = info;
            else
                subscriptionsInfo.Add( id, info );
            var isPaid = info.isFreeTrial() != Result.True;
            var expirationDate = info.getExpireDate();
            var subscriptionSave = GetSubscriptionDataFromSave( id );
            var missingPaymentsCount = 0;

            if ( info.isAutoRenewing() == Result.True )
                missingPaymentsCount = GetMissingPaymentsCount( subscriptionSave, productInfo, expirationDate );

            if ( subscriptionSave != null && subscriptionSave.isPaid == isPaid && missingPaymentsCount <= 0 )
                return;

            DumpSubscriptionDataToSave( id, isPaid, expirationDate );

            for ( var i = 0; i < missingPaymentsCount; i++ )
            {
                OnSubscriptionPurchase.Dispatch( new SubscriptionPurchaseData(
                    productInfo,
                    isPaid,
                    i > 0 || subscriptionSave != null && subscriptionSave.isPaid
                ) );
            }
        }

        static bool IsSubscriptionSave( string id )
        {
            return HPlayerPrefs.HasKey( $"{SUBSCRIPTION_SAVE_PATH}{id}" );
        }

        static int GetMissingPaymentsCount( SubscriptionSaveData save,
            IProductInfo productInfo,
            DateTime expirationDate )
        {
            if ( save == null )
                return 0;

            double periodInSeconds = TimeSpan.FromDays( productInfo.SubscriptionPeriod ).TotalSeconds;

            if ( periodInSeconds == 0 )
            {
                HLog.LogError( logPrefix,
                    $"Please fill subscriptionSpecificInfo for product with id: {productInfo.ProductId}\nUse values higher then zero." );
                return 0;
            }

            var expirationDelta = expirationDate - DateTimeUtils.FromTimestamp( save.expirationTimestamp );
            return Mathf.CeilToInt( (float)( ( (int)expirationDelta.TotalSeconds ) / periodInSeconds ) );
        }

        SubscriptionSaveData GetSubscriptionDataFromSave( string id )
        {
            if ( subscriptionCachedSaves.ContainsKey( id ) )
                return subscriptionCachedSaves[id];

            if ( !IsSubscriptionSave( id ) )
                return null;

            subscriptionCachedSaves.Add( id,
                new SecureCustomPP<SubscriptionSaveData>( $"{SUBSCRIPTION_SAVE_PATH}{id}", encryption ) );
            return subscriptionCachedSaves[id];
        }

        void DumpSubscriptionDataToSave( string id, bool isPaid, DateTime expirationDate )
        {
            if ( !subscriptionCachedSaves.ContainsKey( id ) )
                subscriptionCachedSaves.Add( id,
                    new SecureCustomPP<SubscriptionSaveData>( $"{SUBSCRIPTION_SAVE_PATH}{id}", encryption ) );

            var saveData = new SubscriptionSaveData( id )
            {
                isPaid = isPaid,
                expirationTimestamp = expirationDate.ToTimestamp()
            };
            subscriptionCachedSaves[id].Value = saveData;
        }

        bool TryDeleteSave( string id )
        {
            if ( subscriptionCachedSaves.ContainsKey( id ) )
                subscriptionCachedSaves.Remove( id );

            if ( subscriptionsInfo.ContainsKey( id ) )
                subscriptionsInfo.Remove( id );

            if ( !IsSubscriptionSave( id ) )
                return false;

            HPlayerPrefs.DeleteKey( $"{SUBSCRIPTION_SAVE_PATH}{id}" );
            return true;
        }

        bool IsExpired( SubscriptionInfo info )
        {
            if ( info.isExpired() == Result.False ||
                 info.isAutoRenewing() != Result.Unsupported &&
                 !IsTimeNewer( DateTime.UtcNow, info.getExpireDate() ) )
            {
                return false;
            }

            if ( info.isExpired() == Result.Unsupported )
                return IsTimeNewer( DateTime.UtcNow, GetUnsupportedSubscriptionExpirationDate( info ) );

            return true;
        }

        bool IsTimeNewer( DateTime newTime, DateTime oldTime )
        {
            return DateTime.Compare( newTime, oldTime ) > 0;
        }

        static bool IsSubscribed( SubscriptionInfo info )
        {
            return info.isSubscribed() == Result.True || info.isSubscribed() == Result.Unsupported;
        }

        void LogSubscriptions()
        {
            HLog.Log( logPrefix, "UpdateSubscriptions:" );

            foreach ( var keyValuePair in subscriptionsInfo )
            {
                var id = keyValuePair.Key;

                if ( id == null )
                    continue;

                var info = keyValuePair.Value;
                var stringBuilder = new StringBuilder();
                stringBuilder.Append( $"Subscription id: {id}" );
                stringBuilder.Append( $"\n    status: {GetStatus( id )}" );

                if ( info != null )
                {
                    stringBuilder.Append( $"\n    UTC time now: {DateTime.UtcNow}" );
                    stringBuilder.Append( $"\n    purchase date: {info.getPurchaseDate()}" );
                    stringBuilder.Append( $"\n    expirationDate: {info.getExpireDate()}" );
                    stringBuilder.Append( $"\n    remaining time: {info.getRemainingTime()}" );
                    stringBuilder.Append( $"\n    cancel date: {info.getCancelDate()}" );
                    stringBuilder.Append( $"\n    is trial: {info.isFreeTrial()}" );
                    stringBuilder.Append( $"\n    is auto-renewing: {info.isAutoRenewing()}" );
                    stringBuilder.Append( $"\n    is subscribed: {info.isSubscribed()}" );
                    stringBuilder.Append( $"\n    is expired: {info.isExpired()}" );
                    stringBuilder.Append( $"\n    is canceled: {info.isCancelled()}" );
                    stringBuilder.Append( $"\n    subscription period: {info.getSubscriptionPeriod()}" );
                    stringBuilder.Append( $"\n    free trial period: {info.getFreeTrialPeriod()}" );
                    stringBuilder.Append( $"\n    introductory price period: {info.getIntroductoryPricePeriod()}" );

                    stringBuilder.Append(
                        $"\n    introductory price period cycles: {info.getIntroductoryPricePeriodCycles()}" );
                }

                HLog.Log( logPrefix, stringBuilder.ToString() );
            }
        }
#pragma warning restore 0618
    }
}
#endif