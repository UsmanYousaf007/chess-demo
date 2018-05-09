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
			OrderedDictionary<string, StoreItem> skinItems = new OrderedDictionary<string, StoreItem>()
			{
				{"SkinLuxury", new StoreItem {key = "SkinGold", kind = "Skin", displayName = "Luxury", currency2Cost = 500} },
				{"SkinDeapSea", new StoreItem {key = "SkinOcean", kind = "Skin", displayName = "Deap Sea", currency2Cost = 500} },
				{"SkinWood", new StoreItem {key = "SkinWood", kind = "Skin", displayName = "Wood", currency2Cost = 500} },
				{"SkinDracula", new StoreItem {key = "SkinDracula", kind = "Skin", displayName = "Dracular", currency2Cost = 500} },
				{"SkinMoonlight", new StoreItem {key = "SkinMoonlight", kind = "Skin", displayName = "Moonlight", currency2Cost = 500} },
				{"SkinMarble", new StoreItem {key = "SkinMarble", kind = "Skin", displayName = "Marble", currency2Cost = 500} },
				{"SkinHighrise", new StoreItem {key = "SkinHighrise", kind = "Skin", displayName = "Highrise", currency2Cost = 500} },
				{"SkinCrayon", new StoreItem {key = "SkinCrayon", kind = "Skin", displayName = "Crayon", currency2Cost = 500} },
				{"SkinWinter", new StoreItem {key = "SkinWinter", kind = "Skin", displayName = "Winter", currency2Cost = 500} },
				{"SkinJungle", new StoreItem {key = "SkinJungle", kind = "Skin", displayName = "Jungle", currency2Cost = 500} }
			};

			storeSettingsModel.Add("Skin", skinItems);
		}
	}
}
