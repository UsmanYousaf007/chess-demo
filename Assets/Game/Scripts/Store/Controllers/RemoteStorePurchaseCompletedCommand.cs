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

namespace TurboLabz.InstantGame
{
	public class RemoteStorePurchaseCompletedCommand : Command
	{
		// Params
		[Inject] public string remoteProductId { get; set; }

		// Dispatch
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
