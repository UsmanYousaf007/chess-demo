/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using strange.extensions.command.impl;
using System.Collections.Generic;
using strange.extensions.promise.api;

namespace TurboLabz.InstantChess
{
	public class LoadMetaDataCommand : Command
	{ 
        private OrderedDictionary<string, StoreItem> skinItems = new OrderedDictionary<string, StoreItem>()
        {
            {"SkinLuxury", new StoreItem {key = "SkinLuxury", kind = "Skin", displayName = "Luxury", currency2Cost = 500} },
            {"SkinDeapSea", new StoreItem {key = "SkinDeapSea", kind = "Skin", displayName = "Deap Sea", currency2Cost = 500} },
            {"SkinWood", new StoreItem {key = "SkinWood", kind = "Skin", displayName = "Wood", currency2Cost = 500} },
            {"SkinDracula", new StoreItem {key = "SkinDracula", kind = "Skin", displayName = "Dracular", currency2Cost = 500} },
            {"SkinMoonlight", new StoreItem {key = "SkinMoonlight", kind = "Skin", displayName = "Moonlight", currency2Cost = 500} },
            {"SkinMarble", new StoreItem {key = "SkinMarble", kind = "Skin", displayName = "Marble", currency2Cost = 500} },
            {"SkinHighrise", new StoreItem {key = "SkinHighrise", kind = "Skin", displayName = "Highrise", currency2Cost = 500} },
            {"SkinCrayon", new StoreItem {key = "SkinCrayon", kind = "Skin", displayName = "Crayon", currency2Cost = 500} },
            {"SkinWinter", new StoreItem {key = "SkinWinter", kind = "Skin", displayName = "Winter", currency2Cost = 500} },
            {"SkinJungle", new StoreItem {key = "SkinJungle", kind = "Skin", displayName = "Jungle", currency2Cost = 500} }
        };

       private OrderedDictionary<string, StoreItem> buckPacks = new OrderedDictionary<string, StoreItem>()
       {
            {"BuckPackBronze", new StoreItem {key = "BuckPackBronze", kind = "BuckPack", displayName = "Bronze Pack", currency2Payout = 7500, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.bronzepack"} },
            {"BuckPackSilver", new StoreItem {key = "BuckPackSilver", kind = "BuckPack", displayName = "Silver Pack", currency2Payout = 50000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.silverpack"} },
            {"BuckPackGold", new StoreItem {key = "BuckPackGold", kind = "BuckPack", displayName = "Gold Pack", currency2Payout = 150000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.goldpack"} },
            {"BuckPackPlatinum", new StoreItem {key = "BuckPackPlatinum", kind = "BuckPack", displayName = "Platinum Pack", currency2Payout = 250000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.platinumpack"} },
        };

        private const int ADS_MAX_IMPRESSIONS_PER_LOT = 6;
        private const int ADS_SLOT_DEBUG_MINUTES = 2;
        private const int ADS_SLOT_MINUTES = 1440; // 24 hours

        #region LoadMetaData

        [Inject] public IMetaDataModel model { get; set; }
		[Inject] public IStoreService storeService { get; set; }

		public override void Execute()
        {
            model.AddStoreItem("Skin", skinItems);
            model.AddStoreItem("BuckPack", buckPacks);

			IPromise<bool> promise = storeService.Init(model.getRemoteProductIds());
			promise.Then(OnStoreInit);

			Retain();
        }
			
		private void OnStoreInit(bool success)
		{
			if (success) 
			{
				foreach (KeyValuePair<string, StoreItem> item in model.items) 
				{
					StoreItem storeItem = item.Value;
					if (storeItem.remoteProductId != null) 
					{
						storeItem.remoteProductPrice = storeService.GetItemLocalizedPrice (storeItem.remoteProductId);
					}
				}
			}

			Release();
		}
              
        #endregion
	}
}
