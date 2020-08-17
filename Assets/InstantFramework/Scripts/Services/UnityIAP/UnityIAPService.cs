/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using UnityEngine.Purchasing.Security;
using strange.extensions.promise.api;
using strange.extensions.promise.impl;
using TurboLabz.TLUtils;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
	public class UnityIAPService : IStoreListener, IStoreService
    {
        // Services
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

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        IStoreController storeController = null;
        IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
		IPromise<bool> promise = null;
		purchaseProcessState purchaseState = purchaseProcessState.PURCHASE_STATE_NONE;
        IPromise<BackendResult> storePromise = null;

        private Dictionary<string, Product> pendingVerification = new Dictionary<string, Product>();

		enum purchaseProcessState
		{
			PURCHASE_STATE_NONE,
            PURCHASE_STATE_PENDING,
			PURCHASE_STATE_FAIL,
			PURCHASE_STATE_SUCCESS
		}

		public IPromise<bool> Init(Dictionary<string, ProductType> storeProductIds) 
		{
            if (isStoreAvailable())
            {
                return promise;
            }

			// Create a builder, first passing in a suite of Unity provided stores.
			var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());

			// Add Products
			foreach (var id in storeProductIds)
			{
				builder.AddProduct(id.Key, id.Value);
			}

			UnityPurchasing.Initialize(this, builder);

			promise = new Promise<bool>();
			return promise;
		}
        
		public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
		{
			storeController = controller;
            m_StoreExtensionProvider = extensions;

            foreach (var product in controller.products.all)
            {
                if (product.availableToPurchase &&
                    product.receipt != null &&
                    product.definition.type == ProductType.Subscription &&
                    CheckIfProductIsAvailableForSubscriptionManager(product.receipt))
                {
                    var p = new SubscriptionManager(product, null);
                    var info = p.getSubscriptionInfo();

                    LogUtil.Log("Subscription Info: user have active subscription");
                    LogUtil.Log("Subscription Info: next billing date is: " + info.getExpireDate());
                    LogUtil.Log("Subscription Info: is subscribed? " + info.isSubscribed().ToString());
                    LogUtil.Log("Subscription Info: is expired? " + info.isExpired().ToString());
                    LogUtil.Log("Subscription Info: is cancelled? " + info.isCancelled());
                    LogUtil.Log("Subscription Info: is in free trial peroid? " + info.isFreeTrial());
                    LogUtil.Log("Subscription Info: is auto renewing? " + info.isAutoRenewing());
                    LogUtil.Log("Subscription Info: remaining time " + info.getRemainingTime());

                    playerModel.renewDate = info.getExpireDate().ToShortDateString();

                    var expiryTimeStamp = TimeUtil.ToUnixTimestamp(info.getExpireDate());

                    if (expiryTimeStamp > playerModel.subscriptionExipryTimeStamp)
                    {
                        playerModel.subscriptionExipryTimeStamp = expiryTimeStamp;
                        playerModel.subscriptionType = FindRemoteStoreItemShortCode(product.definition.id);
                        updatePlayerDataSignal.Dispatch();
                    }

                    if (info.isCancelled() == Result.True)
                    {
                        var item = metaDataModel.store.items[FindRemoteStoreItemShortCode(product.definition.id)];
                        hAnalyticsService.LogEvent("cancelled", "subscription", $"subscription_{item.displayName.Replace(" ", "_")}",
                            new KeyValuePair<string, object>("store_iap_id", product.transactionID));
                    }

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

			promise.Dispatch(true);
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			promise.Dispatch(false);
		}

		private bool isStoreAvailable()
		{
			return storeController != null;
		}

        public string GetItemLocalizedDisplayName(string storeProductId)
        {
			if (storeController == null) 
			{
				return null;
			}

            Product product = storeController.products.WithID(storeProductId);
            return product != null ? product.metadata.localizedTitle : null;
        }

        public string GetItemCurrencyCode(string storeProductId)
        {
            if (storeController == null)
            {
                return null;
            }

            Product product = storeController.products.WithID(storeProductId);
            return product != null ? product.metadata.isoCurrencyCode : null;
        }

        public decimal GetItemPrice(string storeProductId)
        {
            if (storeController == null)
            {
                return 0;
            }

            Product product = storeController.products.WithID(storeProductId);
            return product != null ? product.metadata.localizedPrice : 0;
        }

        public string GetItemLocalizedPrice(string storeProductId)
		{
			if (storeController == null) 
			{
				return null;
			}

			Product product = storeController.products.WithID(storeProductId);
			return product != null ? product.metadata.localizedPriceString : null;
		}

		public IPromise<BackendResult> BuyProduct(string storeProductId)
        {
            LogUtil.Log("[IAP TEST] Buy product : " + storeProductId);
            storePromise = new Promise<BackendResult>();

            purchaseState = purchaseProcessState.PURCHASE_STATE_NONE;

            if (storeController == null) 
            {
                return null;
            }

            Product product = storeController.products.WithID(storeProductId);
            if (product != null && product.availableToPurchase) 
            {
                storeController.InitiatePurchase(product);
            }

            showIAPProcessingSignal.Dispatch(true, true);

            return storePromise; //purchaseState == purchaseProcessState.PURCHASE_STATE_PENDING;
        }

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
            LogUtil.Log("[IAP TEST] Process Purchase : ");
            bool validPurchase = true; // Presume valid for platforms with no R.V.

#if !UNITY_EDITOR
            // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
            // Prepare the validator with the secrets we prepared in the Editor
            // obfuscation window.
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);

            try
            {
                bool foundProduct = false;
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(e.purchasedProduct.receipt);
                // For informational purposes, we list the receipt(s)
                Debug.Log("Receipt is valid. Contents:");
                Debug.Log("My transactionID Contents:" + e.purchasedProduct.transactionID);

                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);

                    if(e.purchasedProduct.transactionID == productReceipt.transactionID)
                    {
                        foundProduct = true;
                        break;
                    }
                }

                if(!foundProduct)
                {
                    Debug.Log("Invalid receipt, not found in this list > > > > ");
                    validPurchase = false;
                }

                
            }
            catch (IAPSecurityException)
            {
                Debug.Log("Invalid receipt, not unlocking content");
                validPurchase = false;
            }
#endif
#endif

            if (validPurchase)
            {
                purchaseState = purchaseProcessState.PURCHASE_STATE_PENDING;

                if (!pendingVerification.ContainsKey(e.purchasedProduct.transactionID))
                {
                    pendingVerification.Add(e.purchasedProduct.transactionID, e.purchasedProduct);
                }

                var storeItem = metaDataModel.store.items[FindRemoteStoreItemShortCode(e.purchasedProduct.definition.id)];

                long expiryTimeStamp = 0;
                string shortCode = storeItem.key;

                if (e.purchasedProduct.definition.type == ProductType.Subscription)
                {
                    if (CheckIfProductIsAvailableForSubscriptionManager(e.purchasedProduct.receipt))
                    {
                        var subscriptionInfo = new SubscriptionManager(e.purchasedProduct, null).getSubscriptionInfo();

                        expiryTimeStamp = TimeUtil.ToUnixTimestamp(subscriptionInfo.getExpireDate());

                        if (subscriptionInfo.isFreeTrial() == Result.True)
                        {
                            storeItem.currency1Cost = 0;
                            storeItem.productPrice = 0;
                        }
                    }
#if UNITY_EDITOR
                    expiryTimeStamp = backendService.serverClock.currentTimestamp + (10 * 60 * 1000);
#endif
                }

                // Unlock the appropriate content here.
                backendService.VerifyRemoteStorePurchase(e.purchasedProduct.definition.id, 
                                                            e.purchasedProduct.transactionID, 
                                                            e.purchasedProduct.receipt,
                                                            expiryTimeStamp,
                                                            shortCode).Then(OnVerifiedPurchase);

                return PurchaseProcessingResult.Pending;
            }

            return PurchaseProcessingResult.Complete;
        }

        public void OnVerifiedPurchase(BackendResult result, string transactionID)
        {
            if (pendingVerification.ContainsKey(transactionID))
            {
                if (result == BackendResult.SUCCESS)
                {
                    remoteStorePurchaseCompletedSignal.Dispatch(pendingVerification[transactionID].definition.id);

                    if (storePromise != null)
                    {
                        storePromise.Dispatch(BackendResult.PURCHASE_COMPLETE);
                        storePromise = null;
                    }
                    else
                    {
                        LogAutoRenewEvent("completed", pendingVerification[transactionID].definition.id);
                    }
                }
                else
                {
                    //show dailogue
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CONFIRM_DLG);

                    var vo = new ConfirmDlgVO
                    {
                        title = localizationService.Get(LocalizationKey.STORE_PURCHASE_FAILED_VERIFICATION_TITLE),
                        desc = localizationService.Get(LocalizationKey.STORE_PURCHASE_FAILED_VERIFICATION_DESC),
                        yesButtonText = localizationService.Get(LocalizationKey.STORE_PURCHASE_FAILED_VERIFICATION_YES_BUTTON),
                        onClickYesButton = delegate
                        {
                            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                            contactSupportSignal.Dispatch();
                        }
                    };

                    updateConfirmDlgSignal.Dispatch(vo);

                    metaDataModel.store.failedPurchaseTransactionId = transactionID;

                    if (storePromise != null)
                    {
                        storePromise.Dispatch(BackendResult.PURCHASE_FAILED);
                        storePromise = null;
                    }
                    else
                    {
                        LogAutoRenewEvent("failed", pendingVerification[transactionID].definition.id);
                    }
                }

                storeController.ConfirmPendingPurchase(pendingVerification[transactionID]);
                pendingVerification.Remove(transactionID);
            }

            showIAPProcessingSignal.Dispatch(false, false);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
		{
			purchaseState = purchaseProcessState.PURCHASE_STATE_FAIL;

			LogUtil.Log("UnityIAPService - Purchase failed: " + reason);

            //show dailogue
            //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CONFIRM_DLG);

            //var vo = new ConfirmDlgVO
            //{
            //    title = localizationService.Get(LocalizationKey.STORE_PURCHASE_FAILED),
            //    desc = localizationService.Get(reason.ToString()),
            //    yesButtonText = localizationService.Get(LocalizationKey.LONG_PLAY_OK),
            //    onClickYesButton = delegate
            //    {
            //        navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
            //    }
            //};

            //updateConfirmDlgSignal.Dispatch(vo);

            showIAPProcessingSignal.Dispatch(false, false);

            // Do nothing when user cancels
            if (reason == PurchaseFailureReason.UserCancelled)
            {
                if (storePromise != null)
                {
                    storePromise.Dispatch(BackendResult.PURCHASE_CANCEL);
                    storePromise = null;
                }
                else
                {
                    LogAutoRenewEvent("cancelled", product.definition.id);
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

        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public IPromise<BackendResult> RestorePurchases()
        {
#if UNITY_IOS
             storePromise = new Promise<BackendResult>();

            if (storeController == null || m_StoreExtensionProvider == null) 
            {
                return null;
            }

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
            });

            return storePromise;

#endif
            return null;
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

        private bool CheckIfProductIsAvailableForSubscriptionManager(string receipt)
        {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload"))
            {
                Debug.Log("The product receipt does not contain enough information");
                return false;
            }
            var store = (string)receipt_wrapper["Store"];
            var payload = (string)receipt_wrapper["Payload"];

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

        private void LogAutoRenewEvent(string name, string productId)
        {
            var item = metaDataModel.store.items[FindRemoteStoreItemShortCode(productId)];

            if (name.Equals("failed"))
            {
                hAnalyticsService.LogMonetizationEvent(name, item.currency1Cost, "iap_purchase", $"subscription_{item.displayName.Replace(" ", "_")}", "autorenew",
                    new KeyValuePair<string, object>("store_iap_id", metaDataModel.store.failedPurchaseTransactionId));
            }
            else
            {
                hAnalyticsService.LogMonetizationEvent(name, item.currency1Cost, "iap_purchase", $"subscription_{item.displayName.Replace(" ", "_")}", "autorenew");
            }

            if (name.Equals("completed"))
            {
                analyticsService.Event(AnalyticsEventId.subscription_renewed, item.key.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG) ? AnalyticsContext.monthly : AnalyticsContext.yearly);
                GameAnalyticsSDK.GameAnalytics.NewBusinessEvent("USD", item.currency1Cost, "subscription", item.displayName, "default");
            }
        }

        public void UpgardeSubscription(string oldProductId, string newProductId)
        {
            SubscriptionManager.UpdateSubscriptionInGooglePlayStore(
                storeController.products.WithID(oldProductId),
                storeController.products.WithID(newProductId),
                (oldSku, newSku) => {
                    m_StoreExtensionProvider.GetExtension<IGooglePlayStoreExtensions>().UpgradeDowngradeSubscription(oldSku, newSku);
                });
        }
    }
}