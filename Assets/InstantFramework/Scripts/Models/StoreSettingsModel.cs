/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Purchasing;

namespace TurboLabz.InstantFramework
{
    public class StoreSettingsModel : IStoreSettingsModel
    {
        public bool remoteStoreAvailable { get; set; }
        public IDictionary<string, List<StoreItem>> lists { get; set; }
        public IOrderedDictionary<string, StoreItem> items { get; set; }
        public long lastPurchaseAttemptTimestamp { get; set; }
        public string failedPurchaseTransactionId { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            remoteStoreAvailable = false;
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            lists = new Dictionary<string, List<StoreItem>>();
            items = new OrderedDictionary<string, StoreItem>();
            lastPurchaseAttemptTimestamp = 0;
            failedPurchaseTransactionId = "";
        }

        public Dictionary<string, ProductType> getRemoteProductIds ()
        {
            Dictionary<string, ProductType> ids = new Dictionary<string, ProductType>();

            foreach (KeyValuePair<string, StoreItem> item in items) 
			{
                if (item.Value.remoteProductId != null) 
				{
                    var itemType = item.Value.kind;
                    var productType = itemType.Equals(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_SHOP_TAG) ? ProductType.NonConsumable :
                        itemType.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_TAG) ? ProductType.Subscription : ProductType.Consumable;
                    ids.Add(item.Value.remoteProductId, productType);
				}
			}
			return ids;			
		}

        public void Add(string kind, IOrderedDictionary<string, StoreItem> kindItems)
        {
            List<StoreItem> list = new List<StoreItem>();
            foreach (KeyValuePair<string, StoreItem> item in kindItems)
            {
                list.Add(item.Value);
                items.Add(item.Key, item.Value);
            }

            lists.Add(kind,list);
        }

        public StoreItem GetItemBySkinIndex(int skinIndex)
        {
            var item = (from entry in items
                        where entry.Value.skinIndex == skinIndex
                        select entry).FirstOrDefault();

            return item.Value;
        }

        public StoreItem GetVideoByShortCode(string shortCode)
        {
            List<StoreItem> videoItems;

            if (lists.TryGetValue(GSBackendKeys.ShopItem.VIDEO_LESSON_SHOP_ITEMS, out videoItems))
            {
                for (int i = 0; i < videoItems.Count; i++)
                {
                    if (videoItems[i].key == shortCode)
                    {
                        return videoItems[i];
                    }
                }
            }

            return null;
        }

        public StoreItem GetItemByCoinsValue(int value)
        {
            var item = (from i in lists[GSBackendKeys.ShopItem.COINS_SHOP_TAG]
                        where i.currency4Payout == value
                        select i).FirstOrDefault();
            return item;
        }

    }

    //public class BundledItem
    //{
    //    public string Description;
    //    public int quantity;
    // }

    public class StoreItem
    {
        public string key;                      // Identifier
        public State state;                     // Disabled/Enabled
        public string kind;                     // Classification
        public Type type;                       // VGOOD or CURRENCY
        public int maxQuantity;                 // Max quantity allowed to purchase
        public string displayName;              // Name to display in store          
        public string description;              // Description to display in store     
        public int currency1Cost;               // Cost in currency1
        public int currency2Cost;               // Cost in currency2
        public int currency3Cost;               // Cost in currency3
        public long currency4Cost;              // Cost in currency4
        public int currency1Payout;             // Payout in currency1
        public int currency2Payout;             // Payout in currency2
        public int currency3Payout;             // Payout in currency3
        public long currency4Payout;            // Payout in currency4
        public string remoteProductId;          // Remote store product id
        public string remoteProductPrice;        // Remote store product localized price
        public string remoteProductCurrencyCode; // Remote store product currency code
        public decimal productPrice;             // Remote store product prioce in decimal
        public int skinIndex;                   // Skin sort index
        public int pointsRequired;              // Points required to unlock skin
        public IDictionary<string, int> bundledItems;  // Bundled items
        //public IDictionary<string, BundledItem> bundleDescriptions;  // Bundled item descriptions
        public decimal originalPrice;
        public float discountedRatio;
        public string videoUrl;

        public enum State
        {
            ENABLED,
            DISABLED
        }

        public enum Type
        {
            VGOOD,
            CURRENCY
        }

        public StoreItem()
        {
            key = null;
            state = State.ENABLED;
            kind = null;
            type = Type.VGOOD;
            maxQuantity = 1;
            displayName = null;
            description = null;
            currency1Cost = 0;
            currency2Cost = 0;
            currency3Cost = 0;
            currency4Cost = 0;
            currency1Payout = 0;
            currency2Payout = 0;
            currency3Payout = 0;
            currency4Payout = 0;
            remoteProductId = null;
            remoteProductPrice = null;
            remoteProductCurrencyCode = null;
            productPrice = 0;
            bundledItems = null;
            skinIndex = -1;
            pointsRequired = 0;
            originalPrice = 0;
            discountedRatio = 0;
        }
    }
}
