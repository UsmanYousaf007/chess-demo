/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class StoreSettingsModel : IStoreSettingsModel
    {
        public IDictionary<string, List<ShopItem>> lists{ get; set; }
        public IOrderedDictionary<string, ShopItem> items { get; set; }

        public void Reset()
        {
            lists = null;
            items = null;
        }

        public void Initialize()
        {
            lists = new Dictionary<string, List<ShopItem>>();
            items = new OrderedDictionary<string, ShopItem>();
        }

        public List<string> getProductIds ()
        { 
            List<string> ids = new List<string> ();

            foreach (KeyValuePair<string, ShopItem> item in items) 
			{
                if (item.Value.storeProductId != null) 
				{
                    ids.Add(item.Value.storeProductId);
				}
			}
			return ids;			
		}

        public void Add(string kind, IOrderedDictionary<string, ShopItem> kindItems)
        {
            List<ShopItem> list = new List<ShopItem>();
            foreach (KeyValuePair<string, ShopItem> item in kindItems)
            {
                list.Add(item.Value);
                items.Add(item.Key, item.Value);
            }

            lists.Add(kind,list);
        }
    }

    public class ShopItem
    {
        public string state;                // Disabled/Enabled
        public string id;                   // Short Code
        public string kind;                 // Classification
        public string type;                 // VGOOD or CURRENCY
        public string tier;                 // common, rare, epic, legendary
        public int maxQuantity;             // Max quantity allowed to purchase
        public string displayName;          
        public string description;          
        public int currency1Cost;           // Cost in currency1 or payout 
                                            // in case of CURRENCY type
        public int currency2Cost;           // Cost in currency2

        public string storeProductId;       // Remote store product id
    }

    public class SkinShopItem : ShopItem
    {
        public int unlockAtLevel;
    }

    public class CurrencyShopItem : ShopItem
    {
        public string promotionId;
        public float bonusXpPercentage;
        public int hintsCount;
        public float lossRecoveryPercentage;
        public int bonusAmount;
    }

    public class AvatarShopItem : ShopItem
    {
        public int unlockAtLevel;
    }

    public class ChatpackShopItem : ShopItem
    {
        public int unlockAtLevel;           
    }

    public class AvatarBorderShopItem : ShopItem
    {
        public int unlockAtLevel;
    }
}
