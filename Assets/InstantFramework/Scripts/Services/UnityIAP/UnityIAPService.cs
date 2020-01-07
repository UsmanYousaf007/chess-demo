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

        // Dispatch Signals
        [Inject] public RemoteStorePurchaseCompletedSignal remoteStorePurchaseCompletedSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateConfirmDlgSignal updateConfirmDlgSignal { get; set; }
        [Inject] public ContactSupportSignal contactSupportSignal { get; set; }
        [Inject] public ShowProcessingSignal showIAPProcessingSignal { get; set; }

        IStoreController storeController = null;
        IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
		IPromise<bool> promise = null;
		purchaseProcessState purchaseState = purchaseProcessState.PURCHASE_STATE_NONE;

        private Dictionary<string, Product> pendingVerification = new Dictionary<string, Product>();

		enum purchaseProcessState
		{
			PURCHASE_STATE_NONE,
            PURCHASE_STATE_PENDING,
			PURCHASE_STATE_FAIL,
			PURCHASE_STATE_SUCCESS
		}

		public IPromise<bool> Init(List<string> storeProductIds) 
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
				builder.AddProduct(id, ProductType.Subscription);
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
                    LogUtil.Log("[START]===========UNITY IAP SUBSCRITION INFO==============");
                    LogUtil.Log("product id is: " + info.getProductId());
                    LogUtil.Log("purchase date is: " + info.getPurchaseDate());
                    LogUtil.Log("subscription next billing date is: " + info.getExpireDate());
                    LogUtil.Log("is subscribed? " + info.isSubscribed().ToString());
                    LogUtil.Log("is expired? " + info.isExpired().ToString());
                    LogUtil.Log("is cancelled? " + info.isCancelled());
                    LogUtil.Log("product is in free trial peroid? " + info.isFreeTrial());
                    LogUtil.Log("product is auto renewing? " + info.isAutoRenewing());
                    LogUtil.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                    LogUtil.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                    LogUtil.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                    LogUtil.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                    LogUtil.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());
                    LogUtil.Log("[END]===========UNITY IAP SUBSCRITION INFO==============");
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

		public bool BuyProduct(string storeProductId)
        {
			purchaseState = purchaseProcessState.PURCHASE_STATE_NONE;

            if (storeController == null) 
            {
                return false;
            }

            Product product = storeController.products.WithID(storeProductId);
            if (product != null && product.availableToPurchase) 
            {
                storeController.InitiatePurchase(product);
            }

            showIAPProcessingSignal.Dispatch(true, true);
            return purchaseState == purchaseProcessState.PURCHASE_STATE_PENDING;
        }

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{

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

                long expiryTimeStamp = 0;

                if (e.purchasedProduct.definition.type == ProductType.Subscription &&
                    CheckIfProductIsAvailableForSubscriptionManager(e.purchasedProduct.receipt))
                {
                    expiryTimeStamp = TimeUtil.ToUnixTimestamp(
                        new SubscriptionManager(e.purchasedProduct, null)
                        .getSubscriptionInfo()
                        .getExpireDate());
                }

                // Unlock the appropriate content here.
                backendService.VerifyRemoteStorePurchase(e.purchasedProduct.definition.id, 
                                                            e.purchasedProduct.transactionID, 
                                                            e.purchasedProduct.receipt,
                                                            expiryTimeStamp).Then(OnVerifiedPurchase);

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
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CONFIRM_DLG);

            var vo = new ConfirmDlgVO
            {
                title = localizationService.Get(LocalizationKey.STORE_PURCHASE_FAILED),
                desc = localizationService.Get(reason.ToString()),
                yesButtonText = localizationService.Get(LocalizationKey.LONG_PLAY_OK),
                onClickYesButton = delegate
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
                }
            };

            updateConfirmDlgSignal.Dispatch(vo);

            showIAPProcessingSignal.Dispatch(false, false);

            // Do nothing when user cancels
            if (reason == PurchaseFailureReason.UserCancelled) 
			{
				return;
			} 
			else 
			{
				
			}
		}

        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
#if UNITY_IOS

            if (storeController == null || m_StoreExtensionProvider == null) 
            {
                return;
            }

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
            });

#endif
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
    }
}