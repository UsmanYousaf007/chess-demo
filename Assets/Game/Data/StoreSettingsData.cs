/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using strange.extensions.command.impl;

namespace TurboLabz.InstantChess
{
	public class StoreSettingsData : Command
	{
		[Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

		public override void Execute()
		{
			// Skins
			OrderedDictionary<string, StoreItem> skinItems = new OrderedDictionary<string, StoreItem>()
			{
				{"SkinDeapSea", new StoreItem {key = "SkinDeapSea", kind = "Skin", displayName = "Deap Sea", currency2Cost = 500} },
				{"SkinIndiana", new StoreItem {key = "SkinIndiana", kind = "Skin", displayName = "Indiana", currency2Cost = 500} },
				{"SkinDracula", new StoreItem {key = "SkinDracula", kind = "Skin", displayName = "Dracula", currency2Cost = 500} },
				{"SkinMoonlight", new StoreItem {key = "SkinMoonlight", kind = "Skin", displayName = "Moonlight", currency2Cost = 500} },
				{"SkinMarble", new StoreItem {key = "SkinMarble", kind = "Skin", displayName = "Marble", currency2Cost = 500} },
				{"SkinHighrise", new StoreItem {key = "SkinHighrise", kind = "Skin", displayName = "Highrise", currency2Cost = 500} },
				{"SkinCrayon", new StoreItem {key = "SkinCrayon", kind = "Skin", displayName = "Crayon", currency2Cost = 500} },
				{"SkinWinter", new StoreItem {key = "SkinWinter", kind = "Skin", displayName = "Winter", currency2Cost = 500} },
				{"SkinJungle", new StoreItem {key = "SkinJungle", kind = "Skin", displayName = "Jungle", currency2Cost = 500} },
				{"SkinLuxury", new StoreItem {key = "SkinLuxury", kind = "Skin", displayName = "Luxury", currency2Cost = 500} }
			};

			storeSettingsModel.Add("Skin", skinItems);

			// Buck Packs
			OrderedDictionary<string, StoreItem> buckPacks = new OrderedDictionary<string, StoreItem>()
			{
				{"BuckPackBronze", new StoreItem {key = "BuckPackBronze", kind = "BuckPack", displayName = "Bronze Pack", currency2Payout = 7500} },
				{"BuckPackSilver", new StoreItem {key = "BuckPackSilver", kind = "BuckPack", displayName = "Silver Pack", currency2Payout = 50000} },
				{"BuckPackGold", new StoreItem {key = "BuckPackGold", kind = "BuckPack", displayName = "Gold Pack", currency2Payout = 150000} },
				{"BuckPackPlatinum", new StoreItem {key = "BuckPackPlatinum", kind = "BuckPack", displayName = "Platinum Pack", currency2Payout = 250000} },
			};

			storeSettingsModel.Add("BuckPack", buckPacks);
		
		
		}
	}
}
