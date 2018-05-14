/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using strange.extensions.command.impl;
using System.Collections.Generic;
using strange.extensions.promise.api;
using UnityEngine;

namespace TurboLabz.InstantChess
{
	public class LoadMetaDataCommand : Command
	{ 
        private OrderedDictionary<string, StoreItem> skinItems = new OrderedDictionary<string, StoreItem>()
        {
			{"SkinAmazon", new StoreItem {key = "SkinAmazon", kind = "Skin", displayName = "Amazon", currency2Cost = 200} },
			{"SkinCrayon", new StoreItem {key = "SkinCrayon", kind = "Skin", displayName = "Crayon", currency2Cost = 7500} },
			{"SkinIndiana", new StoreItem {key = "SkinIndiana", kind = "Skin", displayName = "Indiana", currency2Cost = 25000} },
			{"SkinMoonlight", new StoreItem {key = "SkinMoonlight", kind = "Skin", displayName = "Moonlight", currency2Cost = 40000} },
			{"SkinMarble", new StoreItem {key = "SkinMarble", kind = "Skin", displayName = "Marble", currency2Cost = 60000} },
			{"SkinHighrise", new StoreItem {key = "SkinHighrise", kind = "Skin", displayName = "Highrise", currency2Cost = 80000} },
			{"SkinDracula", new StoreItem {key = "SkinDracula", kind = "Skin", displayName = "Dracula", currency2Cost = 100000} },
			{"SkinLuxury", new StoreItem {key = "SkinLuxury", kind = "Skin", displayName = "Luxury", currency2Cost = 120000} },
        };

       private OrderedDictionary<string, StoreItem> buckPacks = new OrderedDictionary<string, StoreItem>()
       {
			// test
			//{"BuckPackInAppTest1", new StoreItem {key = "BuckPackInAppTest1", kind = "BuckPack", displayName = "InAppTest1", currency2Payout = 1000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.instantchess.inapptest1"} },
			{"BuckPackStack", new StoreItem {key = "BuckPackStack", kind = "BuckPack", displayName = "Stack", currency2Payout = 7500, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.bronzepack"} },
			{"BuckPackSuitCase", new StoreItem {key = "BuckPackSuitCase", kind = "BuckPack", displayName = "Suit Case", currency2Payout = 50000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.silverpack"} },
			{"BuckPackCrate", new StoreItem {key = "BuckPackCrate", kind = "BuckPack", displayName = "Crate", currency2Payout = 150000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.goldpack"} },
			{"BuckPackVault", new StoreItem {key = "BuckPackVault", kind = "BuckPack", displayName = "Vault", currency2Payout = 250000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.platinumpack"} },
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

            AdSettings adSettings = new AdSettings();
            adSettings.maxImpressionsPerSlot = ADS_MAX_IMPRESSIONS_PER_LOT;
            adSettings.slotMinutes = Debug.isDebugBuild ? ADS_SLOT_DEBUG_MINUTES : ADS_SLOT_MINUTES;

            model.AddAdSettings(adSettings);
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
