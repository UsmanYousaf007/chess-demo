using System.Collections;
using System.Collections.Generic;
using TurboLabz.InstantFramework;
using HUF.Purchases.Runtime.API;
using strange.extensions.promise.api;
using UnityEngine.Purchasing;
using strange.extensions.promise.impl;
using TurboLabz.InstantGame;
using HUF.Purchases.Runtime.API.Data;
using HUF.Purchases.Runtime.Implementation.Data;
using TurboLabz.TLUtils;
using UnityEngine;

public class HUFIAPService //: IStoreService
{
	[Inject] public IBackendService backendService { get; set; }
	[Inject] public ILocalizationService localizationService { get; set; }
	[Inject] public IHAnalyticsService hAnalyticsService { get; set; }
	[Inject] public IAnalyticsService analyticsService { get; set; }

	// Dispatch Signals
	[Inject] public RemoteStorePurchaseCompletedSignal remoteStorePurchaseCompletedSignal { get; set; }
	[Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
	[Inject] public UpdateConfirmDlgSignal updateConfirmDlgSignal { get; set; }
	[Inject] public ContactSupportSignal contactSupportSignal { get; set; }
	[Inject] public ShowProcessingSignal showIAPProcessingSignal { get; set; }

	//Models
	[Inject] public IMetaDataModel metaDataModel { get; set; }
	[Inject] public LoadPromotionSingal loadPromotionSingal { get; set; }
	[Inject] public UpdatePlayerDataSignal updatePlayerDataSignal { get; set; }

    // ModelspurchaseState
    [Inject] public IPlayerModel playerModel { get; set; }


	IPromise<bool> promise = null;
	
	IPromise<BackendResult> storePromise = null;

    private Dictionary<string, ProductInfo> _products = new Dictionary<string, ProductInfo>();
    private Dictionary<string, IProductInfo> _pendingVerification = new Dictionary<string, IProductInfo>();


	public IPromise<bool> Init(Dictionary<string, ProductType> productsDict)
    {
        List<ProductInfo> products = null;
        if (HPurchases.IsInitialized)
        {
            return promise;
        }

        InitEvents();
        products = new List<ProductInfo>();
        foreach (var prod in productsDict)
        {
            var product = CreateProduct(prod.Key, prod.Value);
            products.Add(product);
            _products[prod.Key] = product;
        }
        HPurchases.Init(products);
        promise = new Promise<bool>();
        return promise;
    }

    private void InitEvents()
    {
        HPurchases.OnInitialized += OnInitialized;
        HPurchases.OnPurchaseInit += OnPurchaseInit;
        HPurchases.OnPurchaseSuccess += OnPurchaseSuccess;
        HPurchases.OnPurchaseFailure += OnPurchaseFailure;
        //HPurchases.OnRefundSuccess += PurchasesOnRefundSuccess;
        HPurchases.OnRestoreFailure += OnRestoreFailure;
        HPurchases.OnSubscriptionPurchased += OnSubscriptionPurchased;
        //HPurchases.OnSubscriptionExpired += SubscriptionExpired;
    }

    public void OnInitialized()
	{
        foreach (KeyValuePair<string, ProductInfo> item in  _products)
        {
            string productId = item.Key;
            ProductInfo productInfo = item.Value;
            var product = HPurchases.TryGetStoreProductInfo(productId);
            if (HPurchases.IsProductAvailable(productId) && productInfo.Type==IAPProductType.Subscription
                /*missing checks*/)
            {
                HPurchases.GetSubscriptionStatus(productId);
                LogUtil.Log("Subscription Info: user have active subscription");
                LogUtil.Log("Subscription Info: next billing date is: " + HPurchases.GetSubscriptionExpirationDate(productId));
                LogUtil.Log("Subscription Info: is subscribed? " + HPurchases.IsSubscriptionActive(productId));
                //LogUtil.Log("Subscription Info: is expired? " + info.isExpired().ToString());
                //LogUtil.Log("Subscription Info: is cancelled? " + info.isCancelled());
                LogUtil.Log("Subscription Info: is in free trial peroid? " + HPurchases.IsSubscriptionInTrialMode(productId));
                //LogUtil.Log("Subscription Info: is auto renewing? " + info.isAutoRenewing());
                //LogUtil.Log("Subscription Info: remaining time " + info.getRemainingTime());

                playerModel.renewDate = HPurchases.GetSubscriptionExpirationDate(productId).ToShortDateString();

                var expiryTimeStamp = TimeUtil.ToUnixTimestamp(HPurchases.GetSubscriptionExpirationDate(productId));
                if (expiryTimeStamp > playerModel.subscriptionExipryTimeStamp)
                {
                    playerModel.subscriptionExipryTimeStamp = expiryTimeStamp;
                    playerModel.subscriptionType = FindRemoteStoreItemShortCode(product.definition.id);
                    updatePlayerDataSignal.Dispatch();
                }

//                if (info.isCancelled() == Result.True)
//                {
//                    var item = metaDataModel.store.items[FindRemoteStoreItemShortCode(product.definition.id)];
//                    hAnalyticsService.LogEvent("cancelled", "subscription", $"subscription_{item.displayName.Replace(" ", "_")}",
//                        new KeyValuePair<string, object>("store_iap_id", product.transactionID));
//                }
#if SUBSCRIPTION_TEST
                if (playerModel.subscriptionExipryTimeStamp > 0)
                {
                    playerModel.subscriptionExipryTimeStamp = 1;
                    loadPromotionSingal.Dispatch();
                    updatePlayerDataSignal.Dispatch();
                }
#endif
            }
        }
        if (promise != null)
        {
            promise.Dispatch(true);
        }
    }

	public void OnPurchaseInit(IProductInfo product)
	{
       
    }

    public void OnSubscriptionPurchased(ISubscriptionPurchaseData purchaseData)
    {
    }


    public void OnPurchaseSuccess(IProductInfo product, TransactionType transactionType, PriceConversionData priceConversion, PurchaseReceiptData receiptData)
    {
//#if !UNITY_EDITOR
            // Unity IAP's validation logic is only included on these platforms.
//#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
            // Prepare the validator with the secrets we prepared in the Editor
            // obfuscation window.
            //var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            //    AppleTangle.Data(), Application.identifier);

            //try
            //{
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            //var result = validator.Validate(e.purchasedProduct.receipt);
            // For informational purposes, we list the receipt(s)
            
                Debug.Log("Receipt is valid. Contents:");
                Debug.Log(receiptData.receipt.TransactionID);
                Debug.Log(product.ProductId);
        
                //foreach (IPurchaseReceipt productReceipt in result)
                //{
                //    Debug.Log(productReceipt.productID);
                //    Debug.Log(productReceipt.purchaseDate);
                //    Debug.Log(productReceipt.transactionID);
                //}
            //}
            //catch (IAPSecurityException)
            //{
            //    Debug.Log("Invalid receipt, not unlocking content");
            //    validPurchase = false;
            //}
//#endif
//#endif


            if (!_pendingVerification.ContainsKey(receiptData.receipt.TransactionID))
            {
                _pendingVerification.Add(receiptData.receipt.TransactionID, product);
            }

            var storeItem = metaDataModel.store.items[FindRemoteStoreItemShortCode(product.ProductId)];

            long expiryTimeStamp = 0;
            string shortCode = storeItem.key;

            if (product.Type == IAPProductType.Subscription)
            {
                //if (CheckIfProductIsAvailableForSubscriptionManager(receiptData.receipt.Payload, receiptData.receipt.Store))
                //{
                expiryTimeStamp = TimeUtil.ToUnixTimestamp(HPurchases.GetSubscriptionExpirationDate(product.ProductId));

                if (HPurchases.IsSubscriptionInTrialMode(product.ProductId))
                {
                    storeItem.currency1Cost = 0;
                    storeItem.productPrice = 0;
                }
                //}
    #if UNITY_EDITOR
                expiryTimeStamp = backendService.serverClock.currentTimestamp + (10 * 60 * 1000);
    #endif
            }

        //// Unlock the appropriate content here.
        //backendService.VerifyRemoteStorePurchase(e.purchasedProduct.definition.id,
        //                                            e.purchasedProduct.transactionID,
        //                                            e.purchasedProduct.receipt,
        //                                            expiryTimeStamp,
        //                                            shortCode).Then(OnVerifiedPurchase);

        //return PurchaseProcessingResult.Pending;

    }

	public void OnPurchaseFailure(IProductInfo productInfo, PurchaseFailureType failureType)
	{
        var product = HPurchases.TryGetStoreProductInfo(productInfo.ProductId);
        LogUtil.Log("UnityIAPService - Purchase failed: " + failureType.ToString());

        showIAPProcessingSignal.Dispatch(false, false);

        // Do nothing when user cancels
        if (failureType == PurchaseFailureType.UserCancelled)
        {
            if (storePromise != null)
            {
                storePromise.Dispatch(BackendResult.PURCHASE_CANCEL);
                storePromise = null;
            }
            else
            {
                
                LogAutoRenewEvent("cancelled", productInfo.ProductId);
            }
            return;
        }
        else
        {
            metaDataModel.store.failedPurchaseTransactionId = product.transactionID;
            if (storePromise != null)
            {
                storePromise.Dispatch(BackendResult.PURCHASE_FAILED);
                storePromise = null;
            }
            else
            {
                LogAutoRenewEvent("failed", product.definition.id);
            }
        }
    }

    public string GetItemLocalizedPrice(string storeProductId)
    {
        if (!HPurchases.IsInitialized)
        {
            return null;
        }

        bool productAvailable = HPurchases.IsProductAvailable(storeProductId);
        return productAvailable ? HPurchases.GetLocalizedPrice(storeProductId, true) : null;
    }

    public decimal GetItemPrice(string storeProductId)
    {
        if (!HPurchases.IsInitialized)
        {
            return 0;
        }

        bool productAvailable = HPurchases.IsProductAvailable(storeProductId);
        return productAvailable ? HPurchases.GetPrice(storeProductId) : 0;
    }

    public string GetItemCurrencyCode(string storeProductId)
    {
        if (!HPurchases.IsInitialized)
        {
            return null;
        }

        bool productAvailable = HPurchases.IsProductAvailable(storeProductId);
        return productAvailable ? HPurchases.GetCurrencyCode(storeProductId) : null;
    }

    public void UpgardeSubscription(string oldProductId, string newProductId)
    {
        HPurchases.UpdateSubscription(oldProductId, newProductId);
    }

    public IPromise<BackendResult> RestorePurchases()
    {
        if (!HPurchases.IsInitialized)
        {
            return null;
        }
#if UNITY_IOS
        HPurchases.RestorePurchases();
        return new Promise<BackendResult>();
#endif
        return null;
    }

    public void OnRestoreFailure()
    { }

    public IPromise<BackendResult> BuyProduct(string storeProductId)
	{
		storePromise = new Promise<BackendResult>();

		if (!HPurchases.IsInitialized)
		{
			return null;
		}

        if(HPurchases.IsProductAvailable(storeProductId))
        {
			HPurchases.BuyProduct(storeProductId);
        }

		showIAPProcessingSignal.Dispatch(true, true);

		return storePromise;
	}

	private IAPProductType ConvertUnityProdTypeToHUFProdType(ProductType productType)
	{
        switch (productType)
        {
			case ProductType.Consumable:
				return IAPProductType.Consumable;

			case ProductType.NonConsumable:
				return IAPProductType.NonConsumable;

			case ProductType.Subscription:
				return IAPProductType.Subscription;

			default:
				return IAPProductType.Consumable;
		}
    }

	private ProductInfo CreateProduct(string productId, ProductType type)
	{
		IAPProductType productType = ConvertUnityProdTypeToHUFProdType(type);
		ProductInfo product = new ProductInfo(productType, productId);
		return product;
	}

    private void LogAutoRenewEvent(string name, string productId)
    {
        var item = metaDataModel.store.items[FindRemoteStoreItemShortCode(productId)];

        if (!item.kind.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG))
        {
            return;
        }

        if (name.Equals("failed"))
        {
            hAnalyticsService.LogMonetizationEvent(name, item.currency1Cost, "iap_purchase", $"subscription_{item.displayName.Replace(" ", "_")}", "autorenew", item.key,
                new KeyValuePair<string, object>("store_iap_id", metaDataModel.store.failedPurchaseTransactionId));
        }
        else
        {
            hAnalyticsService.LogMonetizationEvent(name, item.currency1Cost, "iap_purchase", $"subscription_{item.displayName.Replace(" ", "_")}", "autorenew", item.key);
        }

        if (name.Equals("completed"))
        {
            analyticsService.Event(AnalyticsEventId.subscription_renewed, item.key.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG) ? AnalyticsContext.monthly : AnalyticsContext.yearly);
            GameAnalyticsSDK.GameAnalytics.NewBusinessEvent("USD", item.currency1Cost, "subscription", item.displayName, "default");
        }
    }

    private string FindRemoteStoreItemShortCode(string remoteId)
    {
        foreach (KeyValuePair<string, StoreItem> item in metaDataModel.store.items)
        {
            StoreItem storeItem = item.Value;
            if (storeItem.remoteProductId == remoteId)
            {
                return item.Key;
            }
        }

        return null;
    }

    private bool CheckIfProductIsAvailableForSubscriptionManager(string payload, string store)
    {
        if (string.IsNullOrEmpty(store) || !string.IsNullOrEmpty(payload))
        {
            Debug.Log("The product receipt does not contain enough information");
            return false;
        }

        if (payload != null)
        {
            switch (store)
            {
                case GooglePlay.Name:
                    {
                        var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                        if (!payload_wrapper.ContainsKey("json"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                            return false;
                        }
                        var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                        if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                            return false;
                        }
                        var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                        var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                        if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial"))
                        {
                            Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                            return false;
                        }
                        return true;
                    }
                case AppleAppStore.Name:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
        return false;
    }
}
