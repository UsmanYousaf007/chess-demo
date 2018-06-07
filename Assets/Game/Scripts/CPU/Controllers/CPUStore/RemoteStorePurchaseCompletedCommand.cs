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
	public class RemoteStorePurchaseCompletedCommand : Command
	{
		// Params
		[Inject] public string remoteProductId { get; set; }

		// Dispatch
		[Inject] public SavePlayerSignal savePlayerSignal { get; set; }
		[Inject] public UpdatePlayerBucksDisplaySignal updatePlayerBucksDisplaySignal { get; set; }

		// Models
		[Inject] public IMetaDataModel metaDataModel { get; set; }
		[Inject] public IPlayerModel playerModel { get; set; }

		public override void Execute()
		{
			StoreItem item = FindRemoteStoreItem(remoteProductId);
			if (item == null) 
			{
				return;
			}

            #if UNITY_EDITOR
            // In editor, backend will not verify the purchase so these bucks 
            // will only update locally
            playerModel.bucks += item.currency2Payout;
            LogUtil.Log("Bucks purchase in Unity Editor is not verified and will not update player bucks on the server.", "yellow");
            #endif

			updatePlayerBucksDisplaySignal.Dispatch(playerModel.bucks);
		}

		private StoreItem FindRemoteStoreItem(string remoteId)
		{
			foreach (KeyValuePair<string, StoreItem> item in metaDataModel.store.items) 
			{
				StoreItem storeItem = item.Value;
				if (storeItem.remoteProductId == remoteId) 
				{
					return storeItem;
				}
			}

			return null;
		}
	}
}
