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
			{"SkinSlate", new StoreItem {key = "SkinSlate", kind = "Skin", displayName = "Slate", state = StoreItem.State.DISABLED} },				// factory default
			{"SkinDeepSea", new StoreItem {key = "SkinDeepSea", kind = "Skin", displayName = "Deep Sea", state = StoreItem.State.DISABLED} },		// factory default
			{"SkinIndiana", new StoreItem {key = "SkinIndiana", kind = "Skin", displayName = "Indiana", currency2Cost = 200} },
			{"SkinHighrise", new StoreItem {key = "SkinHighrise", kind = "Skin", displayName = "Highrise", currency2Cost = 7500} },
			{"SkinMarble", new StoreItem {key = "SkinMarble", kind = "Skin", displayName = "Marble", currency2Cost = 25000} },
			{"SkinCrayon", new StoreItem {key = "SkinCrayon", kind = "Skin", displayName = "Crayon", currency2Cost = 40000} },
			{"SkinMoonlight", new StoreItem {key = "SkinMoonlight", kind = "Skin", displayName = "Moonlight", currency2Cost = 60000} },
			{"SkinAmazon", new StoreItem {key = "SkinAmazon", kind = "Skin", displayName = "Amazon", currency2Cost = 80000} },
			{"SkinDracula", new StoreItem {key = "SkinDracula", kind = "Skin", displayName = "Dracula", currency2Cost = 100000} },
			{"SkinLuxury", new StoreItem {key = "SkinLuxury", kind = "Skin", displayName = "Luxury", currency2Cost = 120000} },
        };

       private OrderedDictionary<string, StoreItem> buckPacks = new OrderedDictionary<string, StoreItem>()
       {
			// test
			//{"BuckPackInAppTest1", new StoreItem {key = "BuckPackInAppTest1", kind = "BuckPack", displayName = "InAppTest1", currency2Payout = 1000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.instantchess.inapptest1"} },
			{"BuckPackStack", new StoreItem {key = "BuckPackStack", kind = "BuckPack", displayName = "Stack", currency2Payout = 7500, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.instantchess.inapptest1"} },
			{"BuckPackSuitCase", new StoreItem {key = "BuckPackSuitCase", kind = "BuckPack", displayName = "Suit Case", currency2Payout = 50000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.instantchess.inapptest1"} },
			{"BuckPackCrate", new StoreItem {key = "BuckPackCrate", kind = "BuckPack", displayName = "Crate", currency2Payout = 150000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.instantchess.inapptest1"} },
			{"BuckPackVault", new StoreItem {key = "BuckPackVault", kind = "BuckPack", displayName = "Vault", currency2Payout = 250000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.instantchess.inapptest1"} },

			//{"BuckPackStack", new StoreItem {key = "BuckPackStack", kind = "BuckPack", displayName = "Stack", currency2Payout = 7500, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.buckpackstack"} },
			//{"BuckPackSuitCase", new StoreItem {key = "BuckPackSuitCase", kind = "BuckPack", displayName = "Suit Case", currency2Payout = 50000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.buckpacksuitcase"} },
			//{"BuckPackCrate", new StoreItem {key = "BuckPackCrate", kind = "BuckPack", displayName = "Crate", currency2Payout = 150000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.buckpackcrate"} },
			//{"BuckPackVault", new StoreItem {key = "BuckPackVault", kind = "BuckPack", displayName = "Vault", currency2Payout = 250000, type = StoreItem.Type.CURRENCY, remoteProductId = "com.turbolabz.chess.buckpackvault"} },
        };
            
        private const int DEFAULT_STARTING_BUCKS = 100;
        private readonly string[] DEFAULT_VGOODS = { "SkinSlate", "SkinDeepSea" };
		//private readonly string[] DEFAULT_VGOODS = { "SkinSlate", "SkinDeepSea", "SkinAmazon",  "SkinCrayon", "SkinIndiana", "SkinMoonlight", "SkinMarble", "SkinHighrise", "SkinDracula", "SkinLuxury"};


        private const int ADS_REWARD_INCREMENT = 10;
        private const int ADS_MAX_IMPRESSIONS_PER_SLOT = 6;
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
            adSettings.maxImpressionsPerSlot = ADS_MAX_IMPRESSIONS_PER_SLOT;
            adSettings.slotMinutes = Debug.isDebugBuild ? ADS_SLOT_DEBUG_MINUTES : ADS_SLOT_MINUTES;
            adSettings.adsRewardIncrement = ADS_REWARD_INCREMENT;

            model.AddAdSettings(adSettings);

            model.defaultStartingBucks = DEFAULT_STARTING_BUCKS;
            model.defaultVGoods = DEFAULT_VGOODS;
        }
			
		private void OnStoreInit(bool success)
		{
			if (success) 
			{
				model.remoteStoreAvailable = true;

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
