/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.context.api;
using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;
//using UnityEngine.Purchasing.Security;

namespace TurboLabz.InstantFramework
{
	class GooglePurchaseData {
		// INAPP_PURCHASE_DATA
		public string inAppPurchaseData;
		// INAPP_DATA_SIGNATURE
		public string inAppDataSignature;

		[System.Serializable]
		private class GooglePurchaseReceipt {
			public string Payload = null;
		}

		[System.Serializable]
		private class GooglePurchasePayload {
			public string json = null;
			public string signature = null;
		}

		public GooglePurchaseData(string receipt)
		{
			try
			{
				GooglePurchaseReceipt receiptDeserialized = JsonUtility.FromJson<GooglePurchaseReceipt>(receipt);
				Debug.Log("Payload: " + receiptDeserialized.Payload);

				GooglePurchasePayload payloadDeserialized = JsonUtility.FromJson<GooglePurchasePayload>(receiptDeserialized.Payload);
				Debug.Log("Playload json: " + payloadDeserialized.json);

				inAppPurchaseData = payloadDeserialized.json;
				inAppDataSignature = payloadDeserialized.signature;

			}
			catch (System.Exception e)
			{
				Debug.Log ("Failed to deserialize receipt:" + e);
				inAppPurchaseData = "";
				inAppDataSignature = "";
			}
		}
	}

	public class UnityIAPService : IStoreListener, IStoreService
    {
        // Services
        //[Inject] public IBackendService backendService { get; set; }

        private IStoreController storeController = null;
        private Dictionary<string, Product> pendingVerification = new Dictionary<string, Product>();

		public void Init(List<string> storeProductIds) 
		{
            // Bail if store was already set up.
            if (isStoreAvailable())
            {
                return;
            }

			Debug.Log ("**************************************** UnityIAPService Initialize..");

			// Create a builder, first passing in a suite of Unity provided stores.
			var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());

			// Add Products
			foreach (var id in storeProductIds)
			{
				Debug.Log ("**************************************** UnityIAPService Add Product:" + id);
				builder.AddProduct(id, ProductType.Consumable);
			}

			UnityPurchasing.Initialize(this, builder);
		}
        
		public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
		{
			storeController = controller;

			Debug.Log ("**************************************** UnityIAPService OnInitialized.");
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			// TODO: Create not initialized Bucks purchase state on Store/Shop
			Debug.Log ("**************************************** UnityIAPService OnInitialized FAIL." + error);
		}

		public bool isStoreAvailable()
		{
			return storeController != null;
		}
			
		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
		{
            if (Application.platform == RuntimePlatform.Android)
            {
                if (!pendingVerification.ContainsKey(e.purchasedProduct.transactionID))
                {
                    pendingVerification.Add(e.purchasedProduct.transactionID, e.purchasedProduct);
                }
                GooglePurchaseData googlePurchaseData = new GooglePurchaseData (e.purchasedProduct.receipt);

               // backendService.GooglePlayBuyGoods(e.purchasedProduct.transactionID, "", googlePurchaseData.inAppDataSignature, 
               //     googlePurchaseData.inAppPurchaseData, 0).Then(OnGooglePlayBuyGoods);
            }

            // Always send pending to allow gamesparks to verify the purchase.
            //return PurchaseProcessingResult.Pending;
			return PurchaseProcessingResult.Complete;

        }

		/*
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

        public void BuyProduct(string storeProductId)
        {
            Debug.Log ("**************************************** InAPP BuyProduct.." + storeProductId);
            if (storeController == null) 
            {
                return;
            }

            Product product = storeController.products.WithID(storeProductId);
            if (product != null && product.availableToPurchase) 
            {
                storeController.InitiatePurchase(product);
            }
        }

		public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
		{
			// TODO: And in case of purchase fails, show pop up dialog
            Debug.Log("**************************************** UnityIAPService OnPurchaseFailed: " + reason.ToString());

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
