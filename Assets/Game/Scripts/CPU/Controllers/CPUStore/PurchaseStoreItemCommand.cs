﻿/// @license Propriety <http://license.url>
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
		[Inject] public bool clearForPurchase { get; set; }

		// Dispatch Signals
		[Inject] public SavePlayerSignal savePlayerSignal { get; set; }
		[Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
		[Inject] public UpdateStoreBuyDlgSignal updateStoreBuyDlgSignal { get; set; }
		[Inject] public UpdateStoreNotEnoughBucksDlgSignal updateStoreNotEnoughBucksDlgSignal { get; set; }

		// Models
		[Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
		[Inject] public IPlayerModel playerModel { get; set; }

		// Services
		[Inject] public IStoreService storeService { get; set; }

		public override void Execute()
		{
			StoreItem item = storeSettingsModel.items[key];

			if (playerModel.ownsVGood(key) == true)
			{
				// Case item is already owned
				return;
			}

			if (clearForPurchase == true) 
			{
				// Case Player is clear to purchase item
				Purchase(item);
			} 
			else if (playerModel.bucks < item.currency2Cost) 
			{
				// Case Player does not have enough bucks
				StoreItem bestBuckPackOffer = GetBestBuckPackOffer(item.currency2Cost);
				updateStoreNotEnoughBucksDlgSignal.Dispatch(bestBuckPackOffer);
				navigatorEventSignal.Dispatch (NavigatorEvent.SHOW_NOT_ENOUGH_DLG);
			} 
			else 
			{
				// Case Ask Player for purchase confirmation
				updateStoreBuyDlgSignal.Dispatch(item);
				navigatorEventSignal.Dispatch (NavigatorEvent.SHOW_BUY_DLG);
			}
		}

		private void Purchase(StoreItem item)
		{
			if (item.remoteProductId != null) 
			{
				LogUtil.Log ("Purchase REMOTE" + item.displayName, "cyan");
			} 
			else 
			{
				playerModel.bucks -= item.currency2Cost;
				playerModel.vGoods.Add (key);
				savePlayerSignal.Dispatch();
			}
		}

		private StoreItem GetBestBuckPackOffer(int price)
		{
			List<StoreItem> buckPacks = storeSettingsModel.lists["BuckPack"];
			int bucks = playerModel.bucks;

			int i = 0;
			bool found = false;
			while (!found && i < buckPacks.Count) 
			{
				found = (buckPacks[i].currency2Payout + bucks) >= price;
				if (!found) 
				{
					i++;
				}
			}

			return (i >= buckPacks.Count) ? buckPacks[buckPacks.Count-1] : buckPacks[i];
		}
	}
}
