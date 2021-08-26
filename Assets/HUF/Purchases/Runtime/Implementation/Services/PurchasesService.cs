#if UNITY_PURCHASING
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using HUF.Purchases.Runtime.API;
using HUF.Purchases.Runtime.API.Data;
using HUF.Purchases.Runtime.API.Services;
using HUF.Purchases.Runtime.Implementation.Data;
using HUF.Purchases.Runtime.Utils;
using HUF.Purchases.Runtime.Wrappers;
using HUF.Utils.Runtime;
using HUF.Utils.Runtime.Extensions;
using HUF.Utils.Runtime.Logging;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace HUF.Purchases.Runtime.Implementation.Services
{
    public class PurchasesService : IPurchasesService, IStoreListener
    {
#if UNITY_IOS
        const string SUBSCRIPTION_URL =
            "itms-apps://buy.itunes.apple.com/WebObjects/MZFinance.woa/wa/manageSubscriptions";
#endif
        const string CONSUMED = "CONSUMED";

        public event Action<Product[]> OnInitComplete;
        public event Action<Product, IPurchaseReceipt, SubscriptionResponse> OnPurchaseSuccess;
        public event Action<string, PurchaseFailureType> OnPurchaseFailure;
        public event Action<string> OnPurchaseHandleInterrupted;

        static readonly HLogPrefix logPrefix = new HLogPrefix( HPurchases.logPrefix, nameof(PurchasesService) );
        readonly bool isLocalVerificationEnabled;
        readonly bool isHuuugeServerVerificationEnabled;
        readonly float interruptedPaymentWaitTime;

        static IStoreController storeController;
        static IExtensionProvider storeExtensionProvider;

        public string AccountCurrency { get; private set; }

        public bool IsInitialized => storeController != null && storeExtensionProvider != null;

        readonly HuuugeIAPServerValidator serverValidator;
        Coroutine failPurchaseCoroutine;

        public PurchasesService( IEnumerable<IProductInfo> products,
            bool isLocalVerificationEnabled,
            bool isHuuugeServerVerificationEnabled,
            float interruptedPaymentWaitTime,
            HuuugeIAPServerValidator serverValidator )
        {
            this.isLocalVerificationEnabled = isLocalVerificationEnabled;
            this.isHuuugeServerVerificationEnabled = isHuuugeServerVerificationEnabled;
            this.interruptedPaymentWaitTime = interruptedPaymentWaitTime;
            this.serverValidator = serverValidator;
            InitService( products );
        }

        public void UpdateSubscription( string oldProductId, string newProductId )
        {
            var oldProduct = storeController.products.WithID( oldProductId );
            var newProduct = storeController.products.WithID( newProductId );
#if UNITY_ANDROID
            if ( oldProduct.receipt == null || oldProduct.definition == null )
            {
                OnPurchaseFailure.Dispatch( newProductId, PurchaseFailureType.OldSubscriptionNotBought );
                return;
            }

            if ( !newProduct.availableToPurchase || newProduct.definition == null )
            {
                OnPurchaseFailure.Dispatch( newProductId, PurchaseFailureType.NewSubscriptionNotAvailable );
                return;
            }

            SubscriptionManager.UpdateSubscriptionInGooglePlayStore(
                oldProduct,
                newProduct,
                ( oldSku, newSku ) =>
                {
                    storeExtensionProvider.GetExtension<IGooglePlayStoreExtensions>()
                        .UpgradeDowngradeSubscription( oldProduct.definition.storeSpecificId,
                            newProduct.definition.storeSpecificId );
                } );
#elif UNITY_IOS
            Application.OpenURL( SUBSCRIPTION_URL );
            OnPurchaseFailure.Dispatch( newProductId, PurchaseFailureType.PurchasingUnavailable );
#endif
        }

        void InitService( IEnumerable<IProductInfo> products )
        {
            var purchasingModule = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance( purchasingModule );
            AddProductsToBuilder( products, builder );
            UnityPurchasing.Initialize( this, builder );
            HuuugeIAPServerValidator.OnHuuugeServerValidate += HandleHuuugeServerValidate;
        }

        void AddProductsToBuilder( IEnumerable<IProductInfo> products, ConfigurationBuilder builder )
        {
            var storeName = PurchasesUtils.GetStoreName();

            foreach ( var productInfo in products )
            {
                var storeIds = storeName.IsNotEmpty() ? new IDs { { productInfo.ShopId, storeName } } : null;
                builder.AddProduct( productInfo.ProductId, productInfo.Type.GetPurchasingType(), storeIds );
            }
        }

        public void InitiatePurchase( Product product )
        {
            HLog.Log( logPrefix, $"InitiatePurchase: {product.definition.storeSpecificId}" );
            storeController.InitiatePurchase( product );
        }

        public void RestoreTransactions( Action<bool> restoreCallback )
        {
            var appleStore = storeExtensionProvider.GetExtension<IAppleExtensions>();
            appleStore.RestoreTransactions( restoreCallback );
        }

        void IStoreListener.OnInitialized( IStoreController controller, IExtensionProvider extensions )
        {
            HLog.Log( logPrefix, "OnInitialized: PASS" );
            storeController = controller;
            storeExtensionProvider = extensions;

            var products = storeController.products.all.Aggregate( string.Empty,
                ( current, product ) => $"{current}" +
                                        $"id: {product.definition.id}, " +
                                        $"storeId: {product.definition.storeSpecificId}, " +
                                        $"isAvailableToPurchase: {product.availableToPurchase}, " +
                                        $"hasReceipt: {product.hasReceipt}, " +
                                        $"enabled: {product.definition.enabled}\n" );

            if ( storeController.products.all.Length > 0 )
            {
                AccountCurrency = storeController.products.all[0].metadata.isoCurrencyCode;
            }

            HLog.Log( logPrefix, $"Products received: \n{products}" );
            OnInitComplete.Dispatch( storeController.products.all );
        }

        void IStoreListener.OnInitializeFailed( InitializationFailureReason error )
        {
            HLog.Log( logPrefix, $"OnInitializeFailed InitializationFailureReason:{error}" );
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase( PurchaseEventArgs args )
        {
            var productId = args.purchasedProduct.definition.id;
            IPurchaseReceipt productReceipt = null;
            var isValid = true;

            if ( failPurchaseCoroutine != null )
            {
                CoroutineManager.StopCoroutine( failPurchaseCoroutine );
                failPurchaseCoroutine = null;
                HLog.Log( logPrefix, $"Handle interrupted transaction {args.purchasedProduct.transactionID}" );
                OnPurchaseHandleInterrupted.Dispatch( productId );
            }

            HLog.Log( logPrefix,
                $"Purchase Product id: {( args.purchasedProduct.definition == null ? "null" : args.purchasedProduct.definition.id )}, {args.purchasedProduct.transactionID} " );

            if ( isLocalVerificationEnabled && !Application.isEditor )
            {
                isValid = IsValidReceipt( args, out productReceipt );
            }

            if ( isValid )
            {
                if ( isHuuugeServerVerificationEnabled && !Application.isEditor )
                {
                    CoroutineManager.StartCoroutine( serverValidator.Verify( args.purchasedProduct,
                        productReceipt
                    ) );
                    return PurchaseProcessingResult.Pending;
                }
                else
                {
                    HLog.Log( logPrefix, $"Purchase validated. Product id: {args.purchasedProduct.definition.id}" );
                    OnPurchaseSuccess.Dispatch( args.purchasedProduct, productReceipt, null );
                }
            }
            else
            {
                HLog.Log( logPrefix,
                    "Validation failed, can't find such product in the receipt \n " +
                    $"Purchased productId: {productId}, " +
                    $"transactionId: {args.purchasedProduct.transactionID}" );
                OnPurchaseFailure.Dispatch( productId, PurchaseFailureType.SignatureInvalid );
            }

            return PurchaseProcessingResult.Complete;
        }

        void HandleHuuugeServerValidate( ValidatorResponse response )
        {
            if ( response.IsValidPurchase )
            {
                HLog.Log( logPrefix,
                    $"Purchase validated. Product id: {response.product.definition.id}, requestId: {response.requestId}" );
                OnPurchaseSuccess.Dispatch( response.product, response.receipt, response.subscriptionResponse );

                if ( response.Type == ProductType.Consumable )
                {
                    CoroutineManager.StartCoroutine( serverValidator.ConsumeIapRequest( response.requestId ) );
                }

                storeController.ConfirmPendingPurchase( response.product );
                return;
            }

            var type = PurchaseFailureType.Unknown;

            switch ( (HttpStatusCode)response.responseCode )
            {
                case HttpStatusCode.BadRequest:
                    type = PurchaseFailureType.SignatureInvalid;

                    HLog.LogError( logPrefix,
                        "All client call exceptions with not valid request according to api definition, request should be fixed before retrying." );
                    break;
                case HttpStatusCode.Unauthorized:
                    HLog.LogError( logPrefix,
                        "All client calls with missing or invalid value for X-PROJECT-TOKEN header" );
                    type = PurchaseFailureType.SignatureInvalid;
                    break;
                case HttpStatusCode.NotFound:
                    HLog.LogError( logPrefix, "All client calls with iapId which was not known for system" );
                    type = PurchaseFailureType.SignatureInvalid;
                    break;
                case HttpStatusCode.Conflict:
                    HLog.LogError( logPrefix, "All client calls with iapId which is not in proper state" );
                    type = PurchaseFailureType.SignatureInvalid;
                    break;
                case HttpStatusCode.InternalServerError:
                    HLog.LogError( logPrefix,
                        "All cases for server side processing malfunction, request should be retried." );
                    type = PurchaseFailureType.HuuugeIAPServerError;
                    break;
                default:
                    // this can occur when Apple sandbox experiences problems
                    if ( response.responseError.Contains( CONSUMED ) )
                    {
                        HLog.LogError( logPrefix,
                            $"Purchase was already consumed on the server: {response.responseError}" );
                        storeController.ConfirmPendingPurchase( response.product );
                    }
                    else
                    {
                        HLog.LogError( logPrefix, $"Unknown verification error: {response.responseError}" );
                    }

                    break;
            }

            OnPurchaseFailure.Dispatch( response.product.definition.id, type );
        }

        static bool IsValidReceipt( PurchaseEventArgs args, out IPurchaseReceipt productReceipt )
        {
            return IsValidReceipt( args.purchasedProduct, out productReceipt );
        }

        public static bool IsValidReceipt( Product product, out IPurchaseReceipt productReceipt )
        {
            productReceipt = null;
            var isValid = true;
            var productId = product.definition.id;

            HLog.Log( logPrefix,
                $" Process Purchase, ProductId: {productId}, " +
                $"hasReceipt: {product.hasReceipt}, " +
                $"TransactionId: {product.transactionID}" );

            var validator = new CrossPlatformValidator(
                PurchasesTangleWrapper.GooglePlayTangleData,
                PurchasesTangleWrapper.AppleTangleData,
                Application.identifier );

            try
            {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var receipts = validator.Validate( product.receipt );

                productReceipt = receipts.FirstOrDefault( q =>
                    q.productID.Equals( product.definition.storeSpecificId ) );
                isValid = productReceipt != null;

                foreach ( var receipt in receipts )
                {
                    HLog.Log( logPrefix,
                        $"Receipt productId: {receipt.productID}, transactionId: {receipt.transactionID}" );
                }
            }
            catch ( IAPSecurityException )
            {
                HLog.Log( logPrefix, "Invalid receipt, not unlocking content" );
                isValid = false;
            }
            catch ( Exception e )
            {
                HLog.LogError( logPrefix, e.ToString() );
            }

            return isValid;
        }

        void IStoreListener.OnPurchaseFailed( Product product, PurchaseFailureReason failureReason )
        {
            HLog.Log( logPrefix, $"OnPurchaseFailed: {failureReason}. Product Id: {product.definition.id}" );

            if ( failureReason.GetFailureType() == PurchaseFailureType.Unknown )
            {
                failPurchaseCoroutine = CoroutineManager.StartCoroutine( SendFailWithDelay( product, failureReason ) );
                return;
            }

            OnPurchaseFailure.Dispatch( product.definition.id, failureReason.GetFailureType() );
        }

        IEnumerator SendFailWithDelay( Product product, PurchaseFailureReason failureReason )
        {
            yield return new WaitForSecondsRealtime( interruptedPaymentWaitTime );

            failPurchaseCoroutine = null;
            HLog.Log( logPrefix, $"SendFailWithDelay2: {failureReason}. Product Id: {product.definition.id}" );
            OnPurchaseFailure.Dispatch( product.definition.id, failureReason.GetFailureType() );
        }
    }
}
#endif