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

namespace TurboLabz.InstantFramework
{
	public class UnityIAPService : IStoreListener, IStoreService
    {
        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Dispatch Signals
		[Inject] public RemoteStorePurchaseCompletedSignal remoteStorePurchaseCompletedSignal { get; set; }

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
				builder.AddProduct(id, ProductType.Consumable);
			}

			UnityPurchasing.Initialize(this, builder);

			promise = new Promise<bool>();
			return promise;
		}
        
		public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
		{
			storeController = controller;
            m_StoreExtensionProvider = extensions;
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

            return purchaseState == purchaseProcessState.PURCHASE_STATE_PENDING;
        }

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
            purchaseState = purchaseProcessState.PURCHASE_STATE_PENDING;

            if (!pendingVerification.ContainsKey(e.purchasedProduct.transactionID))
            {
                pendingVerification.Add(e.purchasedProduct.transactionID, e.purchasedProduct);
            }

            backendService.VerifyRemoteStorePurchase(e.purchasedProduct.definition.id, 
                                                        e.purchasedProduct.transactionID, 
                                                        e.purchasedProduct.receipt).Then(OnVerifiedPurchase);

            return PurchaseProcessingResult.Pending;
 		}

        public void OnVerifiedPurchase(BackendResult result, string transactionID)
        {
            if (result == BackendResult.SUCCESS)
            {
                // Confirm the pending purchase
                if (pendingVerification.ContainsKey(transactionID))
                {
                    storeController.ConfirmPendingPurchase(pendingVerification[transactionID]);
                    remoteStorePurchaseCompletedSignal.Dispatch(pendingVerification[transactionID].definition.id);

                    pendingVerification.Remove(transactionID);
                }
            }
        }

		public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
		{
			purchaseState = purchaseProcessState.PURCHASE_STATE_FAIL;

			LogUtil.Log("UnityIAPService - Purchase failed: " + reason);

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
	}
}