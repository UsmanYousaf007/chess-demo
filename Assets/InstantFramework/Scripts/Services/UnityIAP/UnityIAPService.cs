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
	}
}