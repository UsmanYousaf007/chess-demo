//using System.Collections;
//using System.Collections.Generic;
//using TurboLabz.InstantFramework;
//using HUF.Purchases.Runtime.API;
//using strange.extensions.promise.api;
//using UnityEngine.Purchasing;
//using strange.extensions.promise.impl;
//using TurboLabz.InstantGame;
//using HUF.Purchases.Runtime.API.Data;
//using HUF.Purchases.Runtime.Implementation.Data;
//using TurboLabz.TLUtils;
//using UnityEngine;

//public class HUFIAPService : IStoreService
//{
//	[Inject] public IBackendService backendService { get; set; }
//	[Inject] public ILocalizationService localizationService { get; set; }
//	[Inject] public IHAnalyticsService hAnalyticsService { get; set; }
//	[Inject] public IAnalyticsService analyticsService { get; set; }

//	// Dispatch Signals
//	[Inject] public RemoteStorePurchaseCompletedSignal remoteStorePurchaseCompletedSignal { get; set; }
//	[Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
//	[Inject] public UpdateConfirmDlgSignal updateConfirmDlgSignal { get; set; }
//	[Inject] public ContactSupportSignal contactSupportSignal { get; set; }
//	[Inject] public ShowProcessingSignal showIAPProcessingSignal { get; set; }

//	//Models
//	[Inject] public IMetaDataModel metaDataModel { get; set; }
//	[Inject] public LoadPromotionSingal loadPromotionSingal { get; set; }
//	[Inject] public UpdatePlayerDataSignal updatePlayerDataSignal { get; set; }

//    // ModelspurchaseState
//    [Inject] public IPlayerModel playerModel { get; set; }

//	IPromise<bool> promise = null;
//	IPromise<BackendResult> storePromise = null;

//    private Dictionary<string, ProductInfo> _products = new Dictionary<string, ProductInfo>();
//    private Dictionary<string, IProductInfo> _pendingVerification = new Dictionary<string, IProductInfo>();


//    public void OnInitialized()
//	{
//        foreach (KeyValuePair<string, ProductInfo> item in  _products)
//        {
//            string productId = item.Key;
//            ProductInfo productInfo = item.Value;
//            var product = HPurchases.TryGetStoreProductInfo(productId);
//            if (product != null)
//            {
//                if (HPurchases.IsProductAvailable(productId) && productInfo.Type == IAPProductType.Subscription && product.receipt!=null
//                    /*missing checks*/)
//                {
//                    HPurchases.GetSubscriptionStatus(productId);
//                    LogUtil.Log("Subscription Info: user have active subscription");
//                    LogUtil.Log("Subscription Info: next billing date is: " + HPurchases.GetSubscriptionExpirationDate(productId));
//                    LogUtil.Log("Subscription Info: is subscribed? " + HPurchases.IsSubscriptionActive(productId));

//                    //LogUtil.Log("Subscription Info: is expired? " + info.isExpired().ToString());
//                    //LogUtil.Log("Subscription Info: is cancelled? " + info.isCancelled());
//                    LogUtil.Log("Subscription Info: is in free trial peroid? " + HPurchases.IsSubscriptionInTrialMode(productId));
//                    //LogUtil.Log("Subscription Info: is auto renewing? " + info.isAutoRenewing());
//                    //LogUtil.Log("Subscription Info: remaining time " + info.getRemainingTime());

//                    playerModel.renewDate = HPurchases.GetSubscriptionExpirationDate(productId).ToShortDateString();

//                    var expiryTimeStamp = TimeUtil.ToUnixTimestamp(HPurchases.GetSubscriptionExpirationDate(productId));
//                    if (expiryTimeStamp > playerModel.subscriptionExipryTimeStamp)
//                    {
//                        playerModel.subscriptionExipryTimeStamp = expiryTimeStamp;
//                        playerModel.subscriptionType = FindRemoteStoreItemShortCode(productId);
//                        updatePlayerDataSignal.Dispatch();
//                    }

//                    //                if (info.isCancelled() == Result.True)
//                    //                {
//                    //                    var item = metaDataModel.store.items[FindRemoteStoreItemShortCode(product.definition.id)];
//                    //                    hAnalyticsService.LogEvent("cancelled", "subscription", $"subscription_{item.displayName.Replace(" ", "_")}",
//                    //                        new KeyValuePair<string, object>("store_iap_id", product.transactionID));
//                    //                }
//#if BUILD_TEST
//                if (playerModel.subscriptionExipryTimeStamp > 0)
//                {
//                    playerModel.subscriptionExipryTimeStamp = 1;
//                    loadPromotionSingal.Dispatch();
//                    updatePlayerDataSignal.Dispatch();
//                }
//#endif
//                }
//            }
//        }
//        if (promise != null)
//        {
//            promise.Dispatch(true);
//        }
//    }

//	public void OnPurchaseInit(IProductInfo product)
//	{

//    }

//    public void OnSubscriptionPurchased(ISubscriptionPurchaseData purchaseData)
//    {
//    }

//    public void OnPurchaseSuccess(IProductInfo product, TransactionType transactionType, PriceConversionData priceConversion, PurchaseReceiptData receiptData)
//    {
//#if !UNITY_EDITOR
//            // Unity IAP's validation logic is only included on these platforms.
//#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX

//            // For informational purposes, we list the receipt(s)

//                Debug.Log("Receipt is valid. Contents:");
//                Debug.Log(receiptData.receipt.TransactionID);
//                Debug.Log(product.ProductId);
//#endif
//#endif


//        if (!_pendingVerification.ContainsKey(receiptData.receipt.TransactionID))
//            {
//                _pendingVerification.Add(receiptData.receipt.TransactionID, product);
//            }

//            var storeItem = metaDataModel.store.items[FindRemoteStoreItemShortCode(product.ProductId)];

//            long expiryTimeStamp = 0;
//            string shortCode = storeItem.key;

//            if (product.Type == IAPProductType.Subscription)
//            {
//                expiryTimeStamp = TimeUtil.ToUnixTimestamp(HPurchases.GetSubscriptionExpirationDate(product.ProductId));
//                if (HPurchases.IsSubscriptionInTrialMode(product.ProductId))
//                {
//                    storeItem.currency1Cost = 0;
//                    storeItem.productPrice = 0;
//                }
//#if UNITY_EDITOR
//                expiryTimeStamp = backendService.serverClock.currentTimestamp + (10 * 60 * 1000);
//#endif
//            }

//        // Unlock the appropriate content here.
//        backendService.VerifyRemoteStorePurchase(product.ProductId,
//                                                    receiptData.receipt.TransactionID,
//                                                    receiptData.rawReceipt,
//                                                    expiryTimeStamp,
//                                                    shortCode).Then(OnVerifiedPurchase);

//    }

//    public void OnVerifiedPurchase(BackendResult result, string transactionID)
//    {
//        if (_pendingVerification.ContainsKey(transactionID))
//        {
//            if (result == BackendResult.SUCCESS)
//            {
//                remoteStorePurchaseCompletedSignal.Dispatch(_pendingVerification[transactionID].ProductId);

//                if (storePromise != null)
//                {
//                    storePromise.Dispatch(BackendResult.PURCHASE_COMPLETE);
//                    storePromise = null;
//                }
//                else
//                {
//                    LogAutoRenewEvent("completed", _pendingVerification[transactionID].ProductId);
//                }
//            }
//            else
//            {
//                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CONFIRM_DLG);

//                var vo = new ConfirmDlgVO
//                {
//                    title = localizationService.Get(LocalizationKey.STORE_PURCHASE_FAILED_VERIFICATION_TITLE),
//                    desc = localizationService.Get(LocalizationKey.STORE_PURCHASE_FAILED_VERIFICATION_DESC),
//                    yesButtonText = localizationService.Get(LocalizationKey.STORE_PURCHASE_FAILED_VERIFICATION_YES_BUTTON),
//                    onClickYesButton = delegate
//                    {
//                        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
//                        contactSupportSignal.Dispatch();
//                    }
//                };

//                updateConfirmDlgSignal.Dispatch(vo);

//                metaDataModel.store.failedPurchaseTransactionId = transactionID;

//                if (storePromise != null)
//                {
//                    storePromise.Dispatch(BackendResult.PURCHASE_FAILED);
//                    storePromise = null;
//                }
//                else
//                {
//                    LogAutoRenewEvent("failed", _pendingVerification[transactionID].ProductId);
//                }
//            }
//            _pendingVerification.Remove(transactionID);
//        }

//        showIAPProcessingSignal.Dispatch(false, false);
//    }

//    public void OnPurchaseFailure(IProductInfo productInfo, PurchaseFailureType failureType)
//	{
//        var product = HPurchases.TryGetStoreProductInfo(productInfo.ProductId);
//        LogUtil.Log("UnityIAPService - Purchase failed: " + failureType.ToString());

//        showIAPProcessingSignal.Dispatch(false, false);

//        // Do nothing when user cancels
//        if (failureType == PurchaseFailureType.UserCancelled)
//        {
//            if (storePromise != null)
//            {
//                storePromise.Dispatch(BackendResult.PURCHASE_CANCEL);
//                storePromise = null;
//            }
//            else
//            {
//                LogAutoRenewEvent("cancelled", productInfo.ProductId);
//            }
//            return;
//        }
//        else
//        {
//            metaDataModel.store.failedPurchaseTransactionId = product.transactionID;
//            if (storePromise != null)
//            {
//                storePromise.Dispatch(BackendResult.PURCHASE_FAILED);
//                storePromise = null;
//            }
//            else
//            {
//                LogAutoRenewEvent("failed", product.definition.id);
//            }
//        }
//    }

//    public IPromise<bool> Init(Dictionary<string, ProductType> productsDict)
//    {
//        List<ProductInfo> products = null;
//        if (HPurchases.IsInitialized)
//        {
//            return promise;
//        }

//        InitEvents();
//        products = new List<ProductInfo>();
//        foreach (var prod in productsDict)
//        {
//            var product = CreateProduct(prod.Key, prod.Value);
//            products.Add(product);
//            _products[prod.Key] = product;
//        }
//        HPurchases.Init(products);
//        promise = new Promise<bool>();
//        return promise;
//    }

//    public string GetItemLocalizedPrice(string storeProductId)
//    {
//        if (!HPurchases.IsInitialized)
//        {
//            return null;
//        }

//        bool productAvailable = HPurchases.IsProductAvailable(storeProductId);
//        return productAvailable ? HPurchases.GetLocalizedPrice(storeProductId, true) : null;
//    }

//    public decimal GetItemPrice(string storeProductId)
//    {
//        if (!HPurchases.IsInitialized)
//        {
//            return 0;
//        }

//        bool productAvailable = HPurchases.IsProductAvailable(storeProductId);
//        return productAvailable ? HPurchases.GetPrice(storeProductId) : 0;
//    }

//    public string GetItemCurrencyCode(string storeProductId)
//    {
//        if (!HPurchases.IsInitialized)
//        {
//            return null;
//        }

//        bool productAvailable = HPurchases.IsProductAvailable(storeProductId);
//        return productAvailable ? HPurchases.GetCurrencyCode(storeProductId) : null;
//    }

//    public void UpgardeSubscription(string oldProductId, string newProductId)
//    {
//        HPurchases.UpdateSubscription(oldProductId, newProductId);
//    }

//    public IPromise<BackendResult> RestorePurchases()
//    {
//        if (!HPurchases.IsInitialized)
//        {
//            return null;
//        }
//#if UNITY_IOS
//        HPurchases.RestorePurchases();
//        return new Promise<BackendResult>();
//#endif
//        return null;
//    }

//    public void OnRestoreFailure()
//    { }

//    public IPromise<BackendResult> BuyProduct(string storeProductId)
//	{
//		storePromise = new Promise<BackendResult>();

//		if (!HPurchases.IsInitialized)
//		{
//			return null;
//		}

//        if(HPurchases.IsProductAvailable(storeProductId))
//        {
//			HPurchases.BuyProduct(storeProductId);
//        }

//		showIAPProcessingSignal.Dispatch(true, true);

//		return storePromise;
//	}

//    private void InitEvents()
//    {
//        HPurchases.OnInitialized += OnInitialized;
//        HPurchases.OnPurchaseInit += OnPurchaseInit;
//        HPurchases.OnPurchaseSuccess += OnPurchaseSuccess;
//        HPurchases.OnPurchaseFailure += OnPurchaseFailure;
//        HPurchases.OnRestoreFailure += OnRestoreFailure;
//        HPurchases.OnSubscriptionPurchased += OnSubscriptionPurchased;
//    }

//    private IAPProductType ConvertUnityProdTypeToHUFProdType(ProductType productType)
//	{
//        switch (productType)
//        {
//			case ProductType.Consumable:
//				return IAPProductType.Consumable;

//			case ProductType.NonConsumable:
//				return IAPProductType.NonConsumable;

//			case ProductType.Subscription:
//				return IAPProductType.Subscription;

//			default:
//				return IAPProductType.Consumable;
//		}
//    }

//	private ProductInfo CreateProduct(string productId, ProductType type)
//	{
//		IAPProductType productType = ConvertUnityProdTypeToHUFProdType(type);
//		ProductInfo product = new ProductInfo(productType, productId);
//		return product;
//	}

//    private void LogAutoRenewEvent(string name, string productId)
//    {
//        var item = metaDataModel.store.items[FindRemoteStoreItemShortCode(productId)];

//        if (!item.kind.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG))
//        {
//            return;
//        }

//        if (name.Equals("failed"))
//        {
//            hAnalyticsService.LogMonetizationEvent(name, item.currency1Cost, "iap_purchase", $"subscription_{item.displayName.Replace(" ", "_")}", "autorenew", item.key,
//                new KeyValuePair<string, object>("store_iap_id", metaDataModel.store.failedPurchaseTransactionId));
//        }
//        else
//        {
//            hAnalyticsService.LogMonetizationEvent(name, item.currency1Cost, "iap_purchase", $"subscription_{item.displayName.Replace(" ", "_")}", "autorenew", item.key);
//        }

//        if (name.Equals("completed"))
//        {
//            analyticsService.Event(AnalyticsEventId.subscription_renewed, item.key.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG) ? AnalyticsContext.monthly : AnalyticsContext.yearly);
//            GameAnalyticsSDK.GameAnalytics.NewBusinessEvent("USD", item.currency1Cost, "subscription", item.displayName, "default");
//        }
//    }

//    private string FindRemoteStoreItemShortCode(string remoteId)
//    {
//        foreach (KeyValuePair<string, StoreItem> item in metaDataModel.store.items)
//        {
//            StoreItem storeItem = item.Value;
//            if (storeItem.remoteProductId == remoteId)
//            {
//                return item.Key;
//            }
//        }

//        return null;
//    }
//}
