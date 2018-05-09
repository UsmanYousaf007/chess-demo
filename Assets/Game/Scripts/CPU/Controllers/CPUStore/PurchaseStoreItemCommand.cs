/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using System;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantChess
{
	public class PurchaseStoreItemCommand : Command
	{
		// Command Params
		[Inject] public string key { get; set; }

		// Dispatch Signals
		[Inject] public SaveGameSignal saveGameSignal { get; set; }

		// Models
		[Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
		[Inject] public IPlayerModel playerModel { get; set; }

		public override void Execute()
		{
			StoreItem item = storeSettingsModel.items [key];
			playerModel.bucks -= item.currency2Cost;
			playerModel.vGoods.Add(key);

			saveGameSignal.Dispatch();
		}
	}
}
