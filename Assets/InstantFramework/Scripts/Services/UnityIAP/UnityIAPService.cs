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
        IStoreController storeController = null;
        Dictionary<string, Product> pendingVerification = new Dictionary<string, Product>();
		IPromise<bool> promise = null;
		purchaseProcessState purchaseState = purchaseProcessState.PURCHASE_STATE_NONE;

		enum purchaseProcessState
		{
			PURCHASE_STATE_NONE,
			PURCHASE_STATE_FAIL,
			PURCHASE_STATE_SUCCESS
		}

		public IPromise<bool> Init(List<string> storeProductIds) 
		{
            if (isStoreAvailable())
            {
                return null;
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
            Product product = storeController.products.WithID(storeProductId);
            return product != null ? product.metadata.localizedTitle : "unassigned";
        }     

        public string GetItemLocalizedPrice(string storeProductId)
		{
			Product product = storeController.products.WithID(storeProductId);
            return product != null ? product.metadata.localizedPriceString : "unassigned";
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

			return purchaseState == purchaseProcessState.PURCHASE_STATE_SUCCESS;
        }

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				if (!pendingVerification.ContainsKey(e.purchasedProduct.transactionID))
				{
					pendingVerification.Add(e.purchasedProduct.transactionID, e.purchasedProduct);
				}

				// TODO: Implement Reciept Verification for Multiplayer
				// GooglePurchaseData googlePurchaseData = new GooglePurchaseData (e.purchasedProduct.receipt);
				// backendService.GooglePlayBuyGoods(e.purchasedProduct.transactionID, "", googlePurchaseData.inAppDataSignature, 
				//     googlePurchaseData.inAppPurchaseData, 0).Then(OnGooglePlayBuyGoods);
			}

			// Always send pending to allow gamesparks to verify the purchase.
			//return PurchaseProcessingResult.Pending;

			purchaseState = purchaseProcessState.PURCHASE_STATE_SUCCESS;

			return PurchaseProcessingResult.Complete;
		}

		/* // TODO: Implement Reciept Verification for Multiplayer
		public void OnGooglePlayBuyGoods(BackendResult result, string transactionID)
		{
			if (result == BackendResult.SUCCESS)
			{
				// Confirm the pending purchase
				if (pendingVerification.ContainsKey(transactionID))
				{
					storeController.ConfirmPendingPurchase(pendingVerification[transactionID]);
					pendingVerification.Remove(transactionID);
				}
			}
		}
		*/

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
