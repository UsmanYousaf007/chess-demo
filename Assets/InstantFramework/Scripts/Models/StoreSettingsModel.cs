/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class StoreSettingsModel : IStoreSettingsModel
    {
		[Inject] public StoreSettingsDataLoadSignal storeSettingsDataLoadSignal { get; set; }

        public IDictionary<string, List<StoreItem>> lists { get; set; }
        public IOrderedDictionary<string, StoreItem> items { get; set; }

		public void Load()
		{
			Reset();
			Initialize();

			storeSettingsDataLoadSignal.Dispatch();
		}

        public void Reset()
        {
            lists = null;
            items = null;
        }

        public void Initialize()
        {
            lists = new Dictionary<string, List<StoreItem>>();
            items = new OrderedDictionary<string, StoreItem>();
        }

        public List<string> getProductIds ()
        { 
            List<string> ids = new List<string> ();

            foreach (KeyValuePair<string, StoreItem> item in items) 
			{
                if (item.Value.storeProductId != null) 
				{
                    ids.Add(item.Value.storeProductId);
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
    }

    public class StoreItem
    {
        public string key;                  // Identifier
		public State state;                	// Disabled/Enabled
        public string kind;                 // Classification
		public Type type;                 	// VGOOD or CURRENCY
        public string tier;                 // common, rare, epic, legendary
        public int maxQuantity;             // Max quantity allowed to purchase
        public string displayName;			// Name to display in store          
        public string description;     		// Description to display in store     
        public int currency1Cost;           // Cost in currency1
        public int currency2Cost;           // Cost in currency2
		public int currency1Payout;			// Payout in currency1
		public int currency2Payout;			// Payout in currency2
        public string storeProductId;       // Remote store product id

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
			tier = null;
			maxQuantity = 1;
			displayName = null;
			description = null;
			currency1Cost = 0;
			currency2Cost = 0;
			currency1Payout = 0;
			currency2Payout = 0;
		}
	}
}
