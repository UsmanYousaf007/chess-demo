#if UNITY_PURCHASING
using System;
using System.Collections.Generic;
using System.Linq;
using HUF.Purchases.Runtime.API;
using HUF.Purchases.Runtime.API.Data;
using HUF.Purchases.Runtime.API.Models;
using HUF.Purchases.Runtime.API.Services;
using HUF.Purchases.Runtime.Implementation.Data;
using HUF.Purchases.Runtime.Implementation.Services;
using HUF.Utils.Runtime._3rdParty.Blowfish;
using HUF.Utils.Runtime.Configs.API;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.PlayerPrefs;
using HUF.Utils.Runtime.PlayerPrefs.SecureTypes;
using HUF.Utils.Runtime.RandomGenerators;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace HUF.Purchases.Runtime.Implementation.Models
{
    public class PurchasesModel : IPurchasesModel
    {
        static readonly HLogPrefix logPrefix = new HLogPrefix( HPurchases.logPrefix, nameof(PurchasesModel) );
        const string SECURE_KEY_ID = "PurchasesModel.randomKey";

        public bool IsInitialized => purchasesService != null && purchasesService.IsInitialized;

        public event Action OnInitialized;
        public event Action<IProductInfo> OnPurchaseInit;

        public event Action<IProductInfo, TransactionType, PriceConversionData, PurchaseReceiptData>
            OnPurchaseSuccess;

        public event Action<IProductInfo, PurchaseFailureType> OnPurchaseFailure;
        public event Action<IProductInfo> OnPurchaseHandleInterrupted;
        public event Action<IProductInfo> OnRefundSuccess;
        public event Action OnRestoreFailure;
        public event Action<ISubscriptionPurchaseData> OnSubscriptionPurchase;
        public event Action<IProductInfo> OnSubscriptionExpired;

        SecureStringArrayPP purchasesAttemptsMade;

        PurchasesConfig purchasesConfig;
        IPurchasesService purchasesService;
        IRefundService refundService;
        ISubscriptionService subscriptionsService;
        IPriceConversionService priceConversionService;

        BlowFish blowFish;
        Product[] loadedProducts;

        string currencyAmount;
        string currency;

        public void Init()
        {
            if ( purchasesService != null )
            {
                HLog.LogWarning( logPrefix, "Purchases Service is already created" );
                return;
            }

            HLog.Log( logPrefix, " Initializing" );
            purchasesConfig = HConfigs.GetConfig<PurchasesConfig>();

            if ( purchasesConfig == null )
            {
                HLog.Log( logPrefix, "PurchasesConfig is not found. Stop initializing" );
                return;
            }

            var useEncryption = purchasesConfig != null && purchasesConfig.IsEncryptionEnabled;
            GenerateBlowFishKey( useEncryption );
            purchasesAttemptsMade = new SecureStringArrayPP( "PurchasesModel.purchasesAttemptsMade", '|', blowFish );

            purchasesService =
                new PurchasesService( purchasesConfig.Products,
                    purchasesConfig.IsLocalVerificationEnabled,
                    purchasesConfig.IsHuuugeServerVerificationEnabled,
                    purchasesConfig.InterruptedPaymentWaitTime);

            refundService =
                new NonConsumablesRefundService( purchasesConfig.Products,
                    purchasesConfig.IsRefundServiceEnabled,
                    blowFish );

            subscriptionsService =
                new SubscriptionService( purchasesConfig.Products, blowFish );
            priceConversionService = new PriceConversionService( purchasesConfig );
            StartListening();
        }

        public Product TryGetStoreProduct( string productId )
        {
            if ( loadedProducts == null || loadedProducts.Length == 0 )
            {
                HLog.LogWarning( logPrefix, "There are no loaded products available" );
                return null;
            }

            var product = loadedProducts.FirstOrDefault( p => p.definition.id == productId );
            return product;
        }

        public void UpdateSubscription(string oldProductId, string newProductId)
        {
            purchasesService.UpdateSubscription( oldProductId, newProductId );
        }

        void StartListening()
        {
            purchasesService.OnInitComplete += ServiceInitComplete;
            purchasesService.OnPurchaseSuccess += ServicePurchaseSuccess;
            purchasesService.OnPurchaseFailure += ServicePurchaseFailure;
            purchasesService.OnPurchaseHandleInterrupted += HandlePurchaseInterruptedHandle;
            refundService.OnRefundSuccess += OnRefundSuccess;
            subscriptionsService.OnSubscriptionPurchase += OnSubscriptionsPurchasedCallback;
            subscriptionsService.OnSubscriptionExpired += OnSubscriptionsExpiredCallback;
        }

        void ServiceInitComplete( Product[] products )
        {
            loadedProducts = products;
            refundService.ProcessProductsLoading( products );
            subscriptionsService.UpdateSubscriptions( products );
            priceConversionService.LoadPriceData( purchasesService );
            OnInitialized.Dispatch();
        }

        void ServicePurchaseSuccess( Product product, IPurchaseReceipt receipt )
        {
            var productId = product.definition.id;

            var transactionType = TryRemoveAttempt( productId )
                ? TransactionType.Purchase
                : TransactionType.Restore;
            var productInfo = TryGetProductInfo( productId );

            if ( productInfo == null )
            {
                HLog.LogError( logPrefix, $"There is no product info for {productId} in {nameof(PurchasesConfig)}." );
                OnPurchaseFailure.Dispatch( null, PurchaseFailureType.ProductUnavailable );
                return;
            }

            if ( productInfo.Type == IAPProductType.NonConsumable &&
                 !refundService.ProcessTransaction( product, receipt ) )
            {
                HLog.LogError( logPrefix, $"Product {productId} is already purchased." );
                OnPurchaseFailure.Dispatch( null, PurchaseFailureType.NonConsumableAlreadyPurchased );
                return;
            }

            if ( productInfo.Type == IAPProductType.Subscription )
            {
                subscriptionsService.UpdateSubscriptions( loadedProducts );
            }

            if ( purchasesConfig.IsDownloadPriceConversionEnabled &&
                 priceConversionService.GetConversionData() == null )
            {
                priceConversionService.OnGetConversionEnd += HandleGetConversionAfterPurchaseEnd;
                priceConversionService.TryGetConversion( product );
                return;
            }

            OnPurchaseSuccess?.Invoke( productInfo,
                transactionType,
                GetPriceConversionData( product.metadata.localizedPrice ),
                new PurchaseReceiptData( product.receipt ) );
        }

        void HandleGetConversionAfterPurchaseEnd( Product product )
        {
            var productId = product.definition.id;
            var productInfo = TryGetProductInfo( productId );

            var transactionType = TryRemoveAttempt( productId )
                ? TransactionType.Purchase
                : TransactionType.Restore;

            priceConversionService.OnGetConversionEnd -= HandleGetConversionAfterPurchaseEnd;

            OnPurchaseSuccess?.Invoke( productInfo,
                transactionType,
                GetPriceConversionData( product.metadata.localizedPrice ),
                new PurchaseReceiptData( product.receipt ) );
        }

        PriceConversionData GetPriceConversionData( decimal localizedPrice )
        {
            return priceConversionService.GetConversionData() != null
                ? new PriceConversionData( priceConversionService.GetConversionData(), localizedPrice )
                : null;
        }

        void OnSubscriptionsPurchasedCallback( ISubscriptionPurchaseData data )
        {
            OnSubscriptionPurchase.Dispatch( data );
        }

        void OnSubscriptionsExpiredCallback( IProductInfo productInfo )
        {
            OnSubscriptionExpired.Dispatch( productInfo );
        }

        void ServicePurchaseFailure( string productId, PurchaseFailureType reason )
        {
            var productInfo = TryGetProductInfo( productId );
            TryRemoveAttempt( productId );
            OnPurchaseFailure.Dispatch( productInfo, reason );
        }

        void HandlePurchaseInterruptedHandle( string productId )
        {
            var productInfo = purchasesConfig.Products.FirstOrDefault( q => q.ProductId == productId );

            if ( productInfo == null )
                return;
            
            OnPurchaseHandleInterrupted.Dispatch( productInfo );
        }

        public bool IsProductAvailable( string productId )
        {
            var product = TryGetStoreProduct( productId );
            return IsProductAvailable( product );
        }

        public bool IsNonConsumableBought( string productId )
        {
            return refundService != null && refundService.IsNonConsumableBought( productId );
        }

        bool IsProductAvailable( Product product )
        {
            return product != null && product.availableToPurchase;
        }

        public string GetLocalizedPrice( string productId, bool isoFormat )
        {
            var product = TryGetStoreProduct( productId );

            if ( IsProductAvailable( product ) )
            {
                return isoFormat
                    ? Decimal.Round( product.metadata.localizedPrice, 2 ) + product.metadata.isoCurrencyCode
                    : product.metadata.localizedPriceString;
            }

            var productInfo = TryGetProductInfo( productId );

            if ( productInfo != null )
            {
                var priceDollars =
                    Math.Round( (float)productInfo.PriceInCents / 100, 2, MidpointRounding.AwayFromZero );
                return isoFormat ? priceDollars + "USD" : "$" + priceDollars;
            }

            return string.Empty;
        }

        public decimal GetPrice( string productId )
        {
            var product = TryGetStoreProduct( productId );
            return product?.metadata.localizedPrice ?? 0m;
        }

        public void BuyProduct( string productId )
        {
            var productInfo = TryGetProductInfo( productId );

            if ( productInfo == null )
            {
                HLog.LogWarning( logPrefix, "BuyProduct FAIL. ProductInfo is null" );
                OnPurchaseFailure.Dispatch( null, PurchaseFailureType.ProductUnavailable );
                return;
            }

            if ( !IsInitialized )
            {
                HLog.LogWarning( logPrefix, "BuyProduct FAIL. Not initialized." );
                OnPurchaseFailure.Dispatch( productInfo, PurchaseFailureType.PurchasingUnavailable );
                return;
            }

            var product = TryGetStoreProduct( productId );

            if ( IsProductAvailable( product ) )
            {
                if ( purchasesConfig.IsDownloadPriceConversionEnabled )
                {
                    priceConversionService.OnGetConversionEnd += HandleGetConversionBeforePurchaseEnd;
                    priceConversionService.TryGetConversion( product );
                    return;
                }

                TryBuyProduct( productId, productInfo, product );
            }
            else
            {
                HLog.LogWarning( logPrefix,
                    "BuyProduct: FAIL. Not purchasing product, " +
                    $"either is not found or is not available for purchase. Product: {productId}" );
                OnPurchaseFailure.Dispatch( productInfo, PurchaseFailureType.ProductUnavailable );
            }
        }

        void HandleGetConversionBeforePurchaseEnd( Product product )
        {
            var productInfo = TryGetProductInfo( product.definition.id );
            priceConversionService.OnGetConversionEnd -= HandleGetConversionBeforePurchaseEnd;
            TryBuyProduct( product.definition.id, productInfo, product );
        }

        void TryBuyProduct( string productId, IProductInfo productInfo, Product product )
        {
            TryAddAttempt( productId );
            OnPurchaseInit.Dispatch( productInfo );
            purchasesService.InitiatePurchase( product );
        }

        public void RestorePurchases()
        {
            if ( !IsInitialized )
            {
                HLog.LogWarning( logPrefix, "RestorePurchases FAIL. Not initialized." );
                OnRestoreFailure.Dispatch();
                return;
            }

            switch ( Application.platform )
            {
                case RuntimePlatform.IPhonePlayer:
                case RuntimePlatform.OSXPlayer:
                    HLog.Log( logPrefix, "RestorePurchases started ..." );
                    purchasesService.RestoreTransactions( RestorePurchasesCallback );
                    break;
                case RuntimePlatform.Android:
                    HLog.LogWarning( logPrefix,
                        "RestorePurchases triggered on Android. " +
                        "This in unexpected behaviour since Android platform restores it automatically." );
                    OnRestoreFailure.Dispatch();
                    break;
                default:
                    HLog.LogWarning( logPrefix,
                        "RestorePurchases FAIL. Not supported on this platform. " +
                        $"Current = {Application.platform}" );
                    OnRestoreFailure.Dispatch();
                    break;
            }
        }

        void RestorePurchasesCallback( bool result )
        {
            if ( !result )
                OnRestoreFailure.Dispatch();
            HLog.Log( logPrefix, $"RTR RestorePurchases result: {result}." );
        }

        IProductInfo TryGetProductInfo( string productId )
        {
            return purchasesConfig != null ? purchasesConfig.GetProductInfo( productId ) : null;
        }

        void TryAddAttempt( string productId )
        {
            purchasesAttemptsMade.AddUnique( productId );
        }

        bool TryRemoveAttempt( string productId )
        {
            var attempts = purchasesAttemptsMade.Value;
            var attemptsList = attempts == null ? new List<string>() : attempts.ToList();

            if ( attemptsList.Remove( productId ) )
            {
                purchasesAttemptsMade.Value = attemptsList.ToArray();
                return true;
            }

            return false;
        }

        public SubscriptionStatus GetSubscriptionStatus( string subscriptionId )
        {
            return subscriptionsService?.GetStatus( subscriptionId ) ?? SubscriptionStatus.Unknown;
        }

        public bool IsSubscriptionInTrialMode( string subscriptionId )
        {
            return subscriptionsService != null && subscriptionsService.IsInTrialMode( subscriptionId );
        }

        public DateTime GetSubscriptionExpirationDate( string subscriptionId )
        {
            return subscriptionsService?.GetExpirationDate( subscriptionId ) ?? DateTime.MinValue;
        }

        public int GetSubscriptionTrialPeriod( string subscriptionId )
        {
            var productInfo = TryGetProductInfo( subscriptionId );
            return productInfo?.SubscriptionTrialPeriod ?? 0;
        }

        public int GetSubscriptionPeriod( string subscriptionId )
        {
            var productInfo = TryGetProductInfo( subscriptionId );
            return productInfo?.SubscriptionPeriod ?? 0;
        }

        public bool IsSubscriptionActive( string subscriptionId )
        {
            return subscriptionsService?.IsSubscriptionActive( subscriptionId ) ?? false;
        }

        void GenerateBlowFishKey( bool useEncryption )
        {
            if ( !useEncryption )
                return;

            var cipherKey = purchasesConfig.BlowFishKey + GetUniqueSecureKey();

            if ( cipherKey.IsNullOrEmpty() )
            {
                HLog.LogWarning( logPrefix, $"Given key: {cipherKey} is invalid! Please Fix key to use encryption." );
                return;
            }

            blowFish = new BlowFish( cipherKey );
        }

        string GetUniqueSecureKey()
        {
            if ( HPlayerPrefs.HasKey( SECURE_KEY_ID ) )
                return HPlayerPrefs.GetString( SECURE_KEY_ID );

            var playerKey = SecureStringGenerator.RandomString( 16 );
            HPlayerPrefs.SetString( SECURE_KEY_ID, playerKey );
            return playerKey;
        }

        public PriceConversionData GetPriceConversionData()
        {
            return priceConversionService.GetConversionData();
        }

        public void TryGetPriceConversionData( Action<PriceConversionResponse> response, string currency )
        {
            priceConversionService.TryGetPriceConversionData( response, currency );
        }

        public string GetCurrencyCode( string productId )
        {
            var product = TryGetStoreProduct( productId );

            if ( IsProductAvailable( product ) )
            {
                return product.metadata.isoCurrencyCode;
            }

            return string.Empty;
        }
    }
}
#endif
