using System;
using System.Collections.Generic;
using HUF.Purchases.Runtime.API.Data;
using HUF.Purchases.Runtime.API.Models;
using HUF.Purchases.Runtime.Implementation.Data;
using HUF.Purchases.Runtime.Implementation.Models;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using HUF.Utils.Runtime.NetworkRequests;
using JetBrains.Annotations;

namespace HUF.Purchases.Runtime.API
{
    public static class HPurchases
    {
        public static readonly HLogPrefix logPrefix = new HLogPrefix( nameof(HPurchases) );

        static IPurchasesModel purchasesModel;

        static IPurchasesModel PurchasesModel
        {
            get
            {
                if ( purchasesModel == null )
                {
#if UNITY_PURCHASING && !UNITY_EDITOR
                    purchasesModel = new PurchasesModel();
#else
                    purchasesModel = new EmptyPurchasesModel();
#endif
                }

                return purchasesModel;
            }
        }

        /// <summary>
        /// Initialization state of the Purchases model.
        /// </summary>
        [PublicAPI]
        public static bool IsInitialized => PurchasesModel.IsInitialized;

        /// <summary>
        /// Occurs when the Purchases model is initialized and ready for purchases processing.
        /// </summary>
        [PublicAPI]
        public static event Action OnInitialized
        {
            add => PurchasesModel.OnInitialized += value;
            remove => PurchasesModel.OnInitialized -= value;
        }

        /// <summary>
        /// Occurs when a purchasing process is started.
        /// <para> Parameter is the item being purchased. </para>
        /// </summary>
        [PublicAPI]
        public static event Action<IProductInfo> OnPurchaseInit
        {
            add => PurchasesModel.OnPurchaseInit += value;
            remove => PurchasesModel.OnPurchaseInit -= value;
        }

        /// <summary>
        /// Occurs when a purchase is successfully processed.
        /// <para> The first parameter is the item being purchased. </para>
        /// <para> The second parameter is a transaction type (initial purchase or restoration). </para>
        /// <para> The third parameter is a price conversion data with current product price already converted (for now only to USD). Can be null if the conversion data is not available. </para>
        /// <para> The fourth parameter is the receipt of the product.</para>
        /// </summary>
        [PublicAPI]
        public static event Action<IProductInfo, TransactionType, PriceConversionData, PurchaseReceiptData>
            OnPurchaseSuccess
            {
                add => PurchasesModel.OnPurchaseSuccess += value;
                remove => PurchasesModel.OnPurchaseSuccess -= value;
            }

        /// <summary>
        /// Occurs when a purchase fails.
        /// <para> The first parameter contains product info.</para>
        /// <para> The second parameter is a failure reason. </para>
        /// </summary>
        [PublicAPI]
        public static event Action<IProductInfo, PurchaseFailureType> OnPurchaseFailure
        {
            add => PurchasesModel.OnPurchaseFailure += value;
            remove => PurchasesModel.OnPurchaseFailure -= value;
        }

        /// <summary>
        /// Occurs when an item is refunded via Google or Apple panel.
        /// <para> A refunded item.</para>
        /// </summary>
        [PublicAPI]
        public static event Action<IProductInfo> OnRefundSuccess
        {
            add => PurchasesModel.OnRefundSuccess += value;
            remove => PurchasesModel.OnRefundSuccess -= value;
        }

        /// <summary>
        /// Occurs when <see cref="RestorePurchases"/> fails.
        /// It can only occur on iOS builds.
        /// </summary>
        [PublicAPI]
        public static event Action OnRestoreFailure
        {
            add => PurchasesModel.OnRestoreFailure += value;
            remove => PurchasesModel.OnRestoreFailure -= value;
        }

        /// <summary>
        /// Occurs when a subscription is bought or renewed.
        /// <para> The parameter contains subscription details. </para>
        /// </summary>
        [PublicAPI]
        public static event Action<ISubscriptionPurchaseData> OnSubscriptionPurchased
        {
            add => PurchasesModel.OnSubscriptionPurchase += value;
            remove => PurchasesModel.OnSubscriptionPurchase -= value;
        }

        /// <summary>
        /// Occurs when a subscription expires.
        /// <para> The parameter contains product details. </para>
        /// </summary>
        [PublicAPI]
        public static event Action<IProductInfo> OnSubscriptionExpired
        {
            add => PurchasesModel.OnSubscriptionExpired += value;
            remove => PurchasesModel.OnSubscriptionExpired -= value;
        }

        /// <summary>
        /// Occurs when interrupted payment on iOS is detected.
        /// <para> The parameter contains product details. </para>
        /// </summary>
        [PublicAPI]
        public static event Action<IProductInfo> OnPurchaseHandleInterrupted
        {
            add => PurchasesModel.OnPurchaseHandleInterrupted += value;
            remove => PurchasesModel.OnPurchaseHandleInterrupted -= value;
        }

        /// <summary>
        /// Initializes the Purchases model.
        /// </summary>
        [PublicAPI]
        public static void Init()
        {
            PurchasesModel.Init();
        }

        /// <summary>
        /// Initializes the Purchases model.
        /// </summary>
        /// <param name="products">A list of custom products to add at runtime.</param>
        /// <param name="replaceCurrentProducts">If this flag is set to true, current products will be removed.</param>
        [PublicAPI]
        public static void Init( IEnumerable<ProductInfo> products, bool replaceCurrentProducts = true )
        {
            if ( HConfigs.HasConfig<PurchasesConfig>() )
            {
                var config = HConfigs.GetConfig<PurchasesConfig>();
                config.ApplyProducts( products, replaceCurrentProducts );
            }
            else
            {
                HLog.LogWarning( logPrefix, "Unable to apply products. Missing purchases config." );
            }

            Init();
        }

#if UNITY_PURCHASING
        /// <summary>
        /// <para>Gets information about the product from the store.</para>
        /// <para>Note: the product might become available during the gameplay.</para>
        /// </summary>
        /// <param name="productId">An ID of a product.</param>
        /// <returns>A product info or null if no product with a given ID is found.</returns>
        [PublicAPI]
        public static UnityEngine.Purchasing.Product TryGetStoreProductInfo( string productId )
        {
#if !UNITY_EDITOR
            return ( (PurchasesModel)PurchasesModel ).TryGetStoreProduct( productId );
#else
            return null;
#endif
        }
#endif

        /// <summary>
        /// Checks if the product with provided Id is available for purchase.
        /// <para> Note: the product might become available during gameplay, so it is ok to check it repeatedly. </para>
        /// </summary>
        [PublicAPI]
        public static bool IsProductAvailable( string productId )
        {
            return PurchasesModel.IsProductAvailable( productId, out IProductInfo _ );
        }

        /// <summary>
        /// Checks if a product is already bought or not. If the product is refunded returns false.
        /// <para> Note: if Purchases model is not initialized returns a cached state. </para>
        /// </summary>
        [PublicAPI]
        public static bool IsNonConsumableBought( string productId )
        {
            return PurchasesModel.IsNonConsumableBought( productId );
        }

        /// <summary>
        /// Gets currency code in ISO 4217 format. Empty string can be returned if the product is not available.
        /// </summary>
        /// <param name="productId">An IAP product ID.</param>
        /// <returns>A currency code in ISO format.</returns>
        [PublicAPI]
        public static string GetCurrencyCode( string productId )
        {
            return PurchasesModel.GetCurrencyCode( productId );
        }

        /// <summary>
        /// Returns the price of the item with provided ID.
        /// <para> isoFormat == true -> returns localized price in "123.45 USD" format. </para>
        /// <para> isoFormat == false -> returns localized price in "$123.45" format (check if font supports all currency symbols).
        /// If there is no symbol representation of the local currency, it returns "ABC 123.45" format .</para>
        /// <para> Note: while the product is not synced with the store, the default USD price set in the config is returned. </para>
        /// </summary>
        [PublicAPI]
        public static string GetLocalizedPrice( string productId, bool isoFormat )
        {
            return PurchasesModel.GetLocalizedPrice( productId, isoFormat );
        }

        /// <summary>
        /// Returns the price of an item with the provided ID
        /// </summary>
        [PublicAPI]
        public static decimal GetPrice( string productId )
        {
            return PurchasesModel.GetPrice( productId );
        }

        /// <summary>
        /// Returns the price (in cents) of an item with the provided ID.
        /// <para> Note: this method should be used only for analytics when a price in dollars/cents is required. </para>
        /// <para> For showing a price under UI use <see cref="GetLocalizedPrice"/>. </para>
        /// </summary>
        [PublicAPI]
        public static int GetPriceInCents( string productId )
        {
            var config = HConfigs.GetConfig<PurchasesConfig>();
            int? priceInCents = 0;

            if ( config != null )
                priceInCents = config.GetProductInfo( productId )?.PriceInCents;
            return priceInCents ?? 0;
        }

        /// <summary>
        /// Invokes purchasing of a product with the provided ID.
        /// </summary>
        [PublicAPI]
        public static void BuyProduct( string productId )
        {
            PurchasesModel.BuyProduct( productId );
        }

        /// <summary>
        /// Restores purchases.
        /// <para> Note: iOS only - on other platforms it does not have any effect.</para>
        /// <para>More info: https://docs.unity3d.com/Manual/UnityIAPRestoringTransactions.html</para>
        /// </summary>
        [PublicAPI]
        public static void RestorePurchases()
        {
            PurchasesModel.RestorePurchases();
        }

        /// <summary>
        /// Checks if the subscription is still active.
        /// </summary>
        [PublicAPI]
        public static bool IsSubscriptionActive( string productId )
        {
            return PurchasesModel.IsSubscriptionActive( productId );
        }

        /// <summary>
        /// Returns information about subscription, whether it is active, trial and so on.
        /// <para> There are <see cref="OnSubscriptionPurchased"/> and <see cref="OnSubscriptionExpired"/> callbacks
        /// which can be used in order to get a notification when the status changes.</para>
        /// </summary>
        [PublicAPI]
        public static SubscriptionStatus GetSubscriptionStatus( string productId )
        {
            return PurchasesModel.GetSubscriptionStatus( productId );
        }

        /// <summary>
        /// Checks if the subscription is in a trial mode.
        /// </summary>
        [PublicAPI]
        public static bool IsSubscriptionInTrialMode( string productId )
        {
            return PurchasesModel.IsSubscriptionInTrialMode( productId );
        }

        /// <summary>
        /// Returns the DateTime of the subscription expiration.
        /// </summary>
        [PublicAPI]
        public static DateTime GetSubscriptionExpirationDate( string productId )
        {
            return PurchasesModel.GetSubscriptionExpirationDate( productId );
        }

        /// <summary>
        /// Returns (in days) when the subscription’s trial expires.
        /// </summary>
        [PublicAPI]
        public static int GetSubscriptionTrialPeriod( string productId )
        {
            return PurchasesModel.GetSubscriptionTrialPeriod( productId );
        }

        /// <summary>
        /// Returns (in days) the subscription’s period.
        /// </summary>
        [PublicAPI]
        public static int GetSubscriptionPeriod( string productId )
        {
            return PurchasesModel.GetSubscriptionPeriod( productId );
        }

        /// <summary>
        /// Returns the price conversion data. May return null if no conversion data is cached or available at the moment.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public static PriceConversionData GetPriceConversionData()
        {
            return PurchasesModel.GetPriceConversionData();
        }

        /// <summary>
        /// Returns the price conversion data asynchronously.
        /// </summary>
        /// <param name="response">A response callback.</param>
        /// <param name="currency">A currency code in ISO format.</param>
        [PublicAPI]
        public static void TryGetPriceConversionData( Action<PriceConversionResponse> response, string currency )
        {
            if ( !IsInitialized )
            {
                response.Dispatch( new PriceConversionResponse( GameServerResponseStatus.NotInitialized,
                    GameServerUtils.UNINITIALIZED_CODE ) );
                return;
            }

            PurchasesModel.TryGetPriceConversionData( response, currency );
        }

        /// <summary>
        /// Changes from one subscription to other.
        /// On iOS opens Apple Store subscriptions page.
        /// </summary>
        [PublicAPI]
        public static void UpdateSubscription( string oldProductId, string newProductId )
        {
            PurchasesModel.UpdateSubscription( oldProductId, newProductId );
        }
    }
}