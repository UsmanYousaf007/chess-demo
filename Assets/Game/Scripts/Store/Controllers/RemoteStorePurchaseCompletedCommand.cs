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

		// Models
		[Inject] public IMetaDataModel metaDataModel { get; set; }
		[Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IAppsFlyerService appsFlyerService { get; set; }


        public override void Execute()
		{
			StoreItem item = FindRemoteStoreItem(remoteProductId);
			if (item == null) 
			{
				return;
			}

            //appsflyer
            Dictionary<string, string> purchaseEvent = new Dictionary<string, string>();
            purchaseEvent.Add(AFInAppEvents.CURRENCY, item.remoteProductCurrencyCode);
            purchaseEvent.Add(AFInAppEvents.REVENUE, item.productPrice.ToString());
            purchaseEvent.Add(AFInAppEvents.QUANTITY, item.maxQuantity.ToString());
            purchaseEvent.Add(AFInAppEvents.CONTENT_ID, item.remoteProductId);

#if !UNITY_EDITOR

            appsFlyerService.TrackRichEvent(AFInAppEvents.PURCHASE, purchaseEvent);
#endif

            var afEvent = "succ_annual_subs";
            if (item.key.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG))
            {
                if (item.currency1Cost == 0)
                {
                    afEvent = "succ_monthly_subs";
                }
                else
                {
                    afEvent = "succ_renew_monthly_subs";
                }
            }
            else if (item.key.Equals(GSBackendKeys.ShopItem.SUBSCRIPTION_ANNUAL_SHOP_TAG) && item.currency1Cost > 0)
            {
                afEvent = "succ_renew_annual_subs";
            }

            appsFlyerService.TrackRichEvent(afEvent);
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
