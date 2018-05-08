/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using strange.extensions.command.impl;

namespace TurboLabz.Chess
{
	public class StoreSettingsData : Command
	{
		[Inject] public IStoreSettingsModel storeSettingsModel { get; set; }

		public override void Execute()
		{
			OrderedDictionary<string, StoreItem> skinItems = new OrderedDictionary<string, StoreItem>()
			{
				{"SkinDefault", new StoreItem {key = "SkinDefault", kind = "Skin", displayName = "Basic", currency2Cost = 500} },
				{"SkinOcean", new StoreItem {key = "SkinOcean", kind = "Skin", displayName = "Army", currency2Cost = 500} }
			};

			storeSettingsModel.Add("Skin", skinItems);
		}
	}
}
