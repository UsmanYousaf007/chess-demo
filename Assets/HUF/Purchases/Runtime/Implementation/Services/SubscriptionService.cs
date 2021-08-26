#if UNITY_PURCHASING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HUF.Purchases.Runtime.API;
using HUF.Purchases.Runtime.API.Data;
using HUF.Purchases.Runtime.API.Services;
using HUF.Purchases.Runtime.Implementation.Data;
using HUF.Purchases.Runtime.Implementation.Models;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime._3rdParty.Blowfish;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.PlayerPrefs;
using HUF.Utils.Runtime.PlayerPrefs.SecureTypes;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.UDP.Common.MiniJSON;

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
        readonly bool isHuuugeServerVerificationEnabled;
        readonly BlowFish encryption;
        readonly IEnumerable<IProductInfo> productsInfo;
        readonly Dictionary<string, SecureCustomPP<SubscriptionSaveData>> cashedSubscriptionSaves;
        readonly Dictionary<string, SubscriptionResponse> subscriptionsResponses;
        readonly HuuugeIAPServerValidator serverValidator;

        internal SubscriptionService( IEnumerable<IProductInfo> productsInfo,
            bool isHuuugeServerVerificationEnabled,
            HuuugeIAPServerValidator serverValidator,
            BlowFish encryption = null )
        {
            this.productsInfo = productsInfo;
            this.isHuuugeServerVerificationEnabled = isHuuugeServerVerificationEnabled;
            this.serverValidator = serverValidator;
            this.encryption = encryption;
            subscriptionsResponses = new Dictionary<string, SubscriptionResponse>();
            cashedSubscriptionSaves = new Dictionary<string, SecureCustomPP<SubscriptionSaveData>>();

            IntervalManager.Instance.EverySecond += () =>
            {
                var values = cashedSubscriptionSaves.Values.ToList();

                foreach ( var value in values )
                {
                    var id = value.Value.Id;

                    if ( GetStatus( id ) == SubscriptionStatus.Expired )
                    {
                        var productInfo = productsInfo.FirstOrDefault( q => q.ProductId == id );
                        TryDeleteSave( id );
                        OnSubscriptionExpired.Dispatch( productInfo );
                    }
                }
            };
        }

        public void UpdateSubscriptions( Product[] products )
        {
            //TO DO: queueing  
            foreach ( var product in products )
            {
                if ( product == null || product.definition.type != ProductType.Subscription )
                    continue;

                HLog.Log( logPrefix,
                    $"UpdateSubscriptions: {product.definition.id} hasReceipt: {product.hasReceipt} HuuugeIAPServerValidator == null: {HuuugeIAPServerValidator.instance == null}" );

                if ( !product.hasReceipt )
                    continue;

                PurchasesService.IsValidReceipt( product, out var productReceipt );

                if ( productReceipt != null && HuuugeIAPServerValidator.instance != null )
                    CoroutineManager.StartCoroutine(
                        HuuugeIAPServerValidator.instance.Verify( product, productReceipt ) );
            }

            if ( Debug.isDebugBuild )
                LogSubscriptions();
        }

        public void UpdateSubscription( Product product,
            SubscriptionResponse subscriptionResponse,
            IPurchaseReceipt receipt )
        {
            var id = product.definition.id;
            var productInfo = productsInfo.FirstOrDefault( q => q.ProductId == id );

            HLog.Log( logPrefix,
                $"UpdateSubscription: {id} {productInfo} IsExpired {subscriptionResponse.IsExpired} CurrentDate {subscriptionResponse.CurrentDate} ExpiresDate {subscriptionResponse.ExpiresDate}" );

            if ( productInfo == null )
                return;

            if ( subscriptionResponse.IsExpired )
            {
                HLog.Log( logPrefix, "UpdateSubscription: Expired" );

                if ( TryDeleteSave( id ) )
                    OnSubscriptionExpired.Dispatch( productInfo );
                return;
            }

            if ( subscriptionsResponses.ContainsKey( id ) )
                subscriptionsResponses[id] = subscriptionResponse;
            else
                subscriptionsResponses.Add( id, subscriptionResponse );
            var isPaid = !subscriptionResponse.isFreeTrial;
            var expirationDate = subscriptionResponse.ExpiresDate;
            var subscriptionSave = GetSubscriptionDataFromSave( id );
            var missingPaymentsCount = 0;

            if ( subscriptionResponse.isAutoRenewing )
                missingPaymentsCount = GetMissingPaymentsCount( subscriptionSave, productInfo, expirationDate );

            HLog.Log( logPrefix,
                $"UpdateSubscription: isPaid {isPaid} expirationDate {expirationDate} missingPaymentsCount {missingPaymentsCount} isAutoRenewing {subscriptionResponse.isAutoRenewing}" );

            //temporary
            OnSubscriptionPurchase.Dispatch( new SubscriptionPurchaseData(
                productInfo,
                isPaid,
                isPaid
            ) );
            DumpSubscriptionDataToSave( id, isPaid, expirationDate, receipt );

            if ( subscriptionSave != null && subscriptionSave.isPaid == isPaid && missingPaymentsCount <= 0 )
                return;

            for ( var i = 0; i < missingPaymentsCount; i++ )
            {
                OnSubscriptionPurchase.Dispatch( new SubscriptionPurchaseData(
                    productInfo,
                    isPaid,
                    i > 0 || subscriptionSave != null && subscriptionSave.isPaid
                ) );
            }
        }

        public bool IsSubscriptionActive( string id )
        {
            return id != null && GetStatus( id ) == SubscriptionStatus.Active;
        }

        public SubscriptionStatus GetStatus( string id )
        {
            if ( id == null )
                return SubscriptionStatus.Unknown;

            if ( subscriptionsResponses == null || !subscriptionsResponses.ContainsKey( id ) )
            {
                var subscriptionSaveData = GetSubscriptionDataFromSave( id );

                if ( subscriptionSaveData == null )
                    return SubscriptionStatus.Unknown;

                if ( subscriptionSaveData.expirationTimestamp >
                     PurchasesDateTimeUtils.CurrentUTCDateTime.ToTimestamp() )
                    return SubscriptionStatus.Active;

                VerifySubscriptionRenewal( id );
                return SubscriptionStatus.Expired;
            }

            var subscriptionsResponse = subscriptionsResponses[id];

            if ( subscriptionsResponse.ExpiresDate.ToTimestamp() >
                 PurchasesDateTimeUtils.CurrentUTCDateTime.ToTimestamp() )
                return subscriptionsResponse.isSubscribed ? SubscriptionStatus.Active : SubscriptionStatus.Unknown;

            VerifySubscriptionRenewal( id );
            return SubscriptionStatus.Expired;
        }

        private void VerifySubscriptionRenewal( string id )
        {
#if UNITY_ANDROID
            if ( !isHuuugeServerVerificationEnabled || Application.isEditor )
                return;

            var subscriptionSaveData = GetSubscriptionDataFromSave( id );

            if ( subscriptionSaveData == null )
                return;

            var product = HPurchases.TryGetStoreProductInfo( id );

            var receipt = new GooglePlayReceipt( null,
                null,
                null,
                subscriptionSaveData.PurchaseToken,
                new DateTime(),
                GooglePurchaseState.Purchased );

            CoroutineManager.StartCoroutine( serverValidator.VerifySubscriptionRenewal( product,
                receipt
            ) );
#endif
        }

        public bool IsInTrialMode( string id )
        {
            if ( subscriptionsResponses == null || !subscriptionsResponses.ContainsKey( id ) )
            {
                var subscriptionSaveData = GetSubscriptionDataFromSave( id );
                return subscriptionSaveData != null && !subscriptionSaveData.isPaid;
            }

            var subscriptionResponse = subscriptionsResponses[id];

            return subscriptionResponse.isFreeTrial &&
                   IsTimeNewer( PurchasesDateTimeUtils.CurrentUTCDateTime, subscriptionResponse.ExpiresDate ) ==
                   false;
        }

        public DateTime GetExpirationDate( string id )
        {
            if ( subscriptionsResponses == null || !subscriptionsResponses.ContainsKey( id ) )
            {
                var subscriptionSaveData = GetSubscriptionDataFromSave( id );

                return subscriptionSaveData != null
                    ? DateTimeOffset.FromUnixTimeSeconds( subscriptionSaveData.expirationTimestamp ).UtcDateTime
                    : PurchasesDateTimeUtils.CurrentUTCDateTime.AddDays( -1 );
            }

            var subscriptionResponse = subscriptionsResponses[id];
            return subscriptionResponse.ExpiresDate;
        }

        /*DateTime GetUnsupportedSubscriptionExpirationDate( SubscriptionInfo info )
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

            if ( subscriptionsResponses.ContainsKey( id ) )
                subscriptionsResponses[id] = info;
            else
                subscriptionsResponses.Add( id, info );
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
        }*/

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
            if ( cashedSubscriptionSaves.ContainsKey( id ) )
                return cashedSubscriptionSaves[id];

            if ( !IsSubscriptionSave( id ) )
                return null;

            cashedSubscriptionSaves.Add( id,
                new SecureCustomPP<SubscriptionSaveData>( $"{SUBSCRIPTION_SAVE_PATH}{id}", encryption ) );
            return cashedSubscriptionSaves[id];
        }

        void DumpSubscriptionDataToSave( string id, bool isPaid, DateTime expirationDate, IPurchaseReceipt receipt )
        {
            if ( !cashedSubscriptionSaves.ContainsKey( id ) )
                cashedSubscriptionSaves.Add( id,
                    new SecureCustomPP<SubscriptionSaveData>( $"{SUBSCRIPTION_SAVE_PATH}{id}", encryption ) );
#if UNITY_ANDROID
            var purchaseToken = ( (GooglePlayReceipt)receipt ).purchaseToken;
#else
            var purchaseToken = String.Empty;
#endif

            var saveData = new SubscriptionSaveData( id, purchaseToken )
            {
                isPaid = isPaid,
                expirationTimestamp = expirationDate.ToTimestamp()
            };
            cashedSubscriptionSaves[id].Value = saveData;
        }

        bool TryDeleteSave( string id )
        {
            if ( cashedSubscriptionSaves.ContainsKey( id ) )
                cashedSubscriptionSaves.Remove( id );

            if ( subscriptionsResponses.ContainsKey( id ) )
                subscriptionsResponses.Remove( id );

            if ( !IsSubscriptionSave( id ) )
                return false;

            HPlayerPrefs.DeleteKey( $"{SUBSCRIPTION_SAVE_PATH}{id}" );
            return true;
        }

        bool IsTimeNewer( DateTime newTime, DateTime oldTime )
        {
            return DateTime.Compare( newTime, oldTime ) > 0;
        }

        void LogSubscriptions()
        {
            HLog.Log( logPrefix, "UpdateSubscriptions:" );

            foreach ( var keyValuePair in subscriptionsResponses )
            {
                var id = keyValuePair.Key;

                if ( id == null )
                    continue;

                var subscriptionResponse = keyValuePair.Value;
                var stringBuilder = new StringBuilder();
                stringBuilder.Append( $"Subscription id: {id}" );

                stringBuilder.Append(
                    $"\n    status: {GetStatus( id )}\n{JsonUtility.ToJson( subscriptionResponse )}" );
                HLog.Log( logPrefix, stringBuilder.ToString() );
            }
        }
#pragma warning restore 0618
    }
}
#endif