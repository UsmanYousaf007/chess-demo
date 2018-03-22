
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace TurboLabz.Gamebet
{
    class GooglePurchaseData {
        // INAPP_PURCHASE_DATA
        public string inAppPurchaseData;
        // INAPP_DATA_SIGNATURE
        public string inAppDataSignature;

        [System.Serializable]
        private struct GooglePurchaseReceipt {
            public string Payload;
        }
        [System.Serializable]
        private struct GooglePurchasePayload {
            public string json;
            public string signature;
        }

        public GooglePurchaseData(string receipt) {
            try {
                var purchaseReceipt = JsonUtility.FromJson<GooglePurchaseReceipt> (receipt);
                var purchasePayload = JsonUtility.FromJson<GooglePurchasePayload> (purchaseReceipt.Payload);
                inAppPurchaseData = purchasePayload.json;
                inAppDataSignature = purchasePayload.signature;
            } catch {
                Debug.Log("Could not parse receipt: " + receipt);
                inAppPurchaseData = "";
                inAppDataSignature = "";
            }
        }
    }
        
    public class InAppPurchase : MonoBehaviour, IStoreListener
    {
        public static InAppPurchase instance;
        private const string productIdPrefix = "com.turbolabz.chessdev.";

        // Services
        [Inject] public IBackendService backendService { get; set; }

        private static IStoreController m_StoreController;
        private static IExtensionProvider m_StoreExtensionProvider;

        public static void CreateInAppPurchaseObject(IShopSettingsModel shopSettings)
        {
            GameObject objectInAppPurchase = new GameObject("InAppPurchase");
            objectInAppPurchase.AddComponent<InAppPurchase>();

            InAppPurchase inAppPurchase = objectInAppPurchase.GetComponent<InAppPurchase>();
            inAppPurchase.Initialize(shopSettings);
        }

        void Start()
        {
            instance = this;
            Debug.Log ("**************************************** InAppPurchase Object Started.");
        }

        private void Initialize(IShopSettingsModel shopSettings)
        {
            if (m_StoreController != null)
            {
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance (StandardPurchasingModule.Instance ());

            // Add Products
            foreach (var item in shopSettings.currencyShopItems)
            {
                if (item.Value.type == GSBackendKeys.ShopItem.SHOP_ITEM_TYPE_CURRENCY)
                {
                    string productId = productIdPrefix + item.Value.id.ToLower();
                    builder.AddProduct(productId, ProductType.Consumable);
                }
            }

            UnityPurchasing.Initialize (this, builder);

            Debug.Log ("**************************************** InAppPurchase Initialize..");
        }

        private bool IsInitialized ()
        {
            return m_StoreController != null;
        }

        public void BuyProduct(string productId)
        {
            string storeProductId = productIdPrefix + productId.ToLower();

            if (IsInitialized ()) {
                Product product = m_StoreController.products.WithID (storeProductId);

                if (product != null && product.availableToPurchase) {
                    m_StoreController.InitiatePurchase (product);
                }
            }
        }

        public void ReportProduct(PurchaseEventArgs args, string productId)
        {
            // A consumable product has been purchased by this user.
            if (String.Equals (args.purchasedProduct.definition.id, productId, StringComparison.Ordinal)) {
                Debug.Log (string.Format ("****************************************   ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            }
            else {
                Debug.Log (string.Format ("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }
        }

        // IStoreListener interface
        public void OnInitialized (IStoreController controller, IExtensionProvider extensions)
        {
            m_StoreController = controller;
            m_StoreExtensionProvider = extensions;

            Product[] product = m_StoreController.products.all;
            for (int i = 0; i < product.Length; i++)
            {
                Debug.Log ("******************   GOOGLE PLAY ITEMS available=" + 
                product[i].availableToPurchase + "definition=" + product[i].definition + "meta=" + product[i].metadata.localizedPriceString + "  descp=  " + product[i].metadata.localizedDescription);
            }
        }

        public string GetItemLocalizedPrice(string productId)
        {
            string storeProductId = productIdPrefix + productId.ToLower();
            Product product = m_StoreController.products.WithID(storeProductId);
            return product.metadata.localizedPriceString;
        }

        // IStoreListener interface
        public void OnInitializeFailed (InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log ("*****************************************  OnInitializeFailed InitializationFailureReason:" + error);
        }

        // IStoreListener interface
        public PurchaseProcessingResult ProcessPurchase (PurchaseEventArgs args)
        {
            var data = new GooglePurchaseData(args.purchasedProduct.receipt);
            Debug.Log ("************************************************************************INAPP   ProcessPurchase---> " + args.purchasedProduct.receipt + "   SIGNATURE->" + data.inAppDataSignature + "   INAPPPURCHASE->" + data.inAppPurchaseData);

            if (Application.platform == RuntimePlatform.Android)
            {
                backendService.GooglePlayBuyVirtualGoods("", data.inAppDataSignature, data.inAppPurchaseData, 0);
            }
            return PurchaseProcessingResult.Complete;
        }

        // IStoreListener interface
        public void OnPurchaseFailed (Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log (string.Format ("***************************************   OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }

    }
}







