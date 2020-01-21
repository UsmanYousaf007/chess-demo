/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using HUF.Analytics.API;
using strange.extensions.command.impl;
using strange.extensions.promise.api;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;

namespace TurboLabz.InstantGame
{
    public class RestorePurchasesCommand : Command
    {
        // Services
        [Inject] public IStoreService storeService { get; set; }
        [Inject] public IMetaDataModel metaDataModel { get; set; }
        [Inject] public INavigatorModel navigatorModel { get; set; }

        private StoreItem item = null;
        private NS pState = null;

        public override void Execute()
        {
            //storeService.RestorePurchases();
            pState = navigatorModel.previousState;

            IPromise<BackendResult> promise = storeService.RestorePurchases();
            if (promise != null)
            {
                promise.Then(OnPromiseReturn);
            }
        }

        private void OnPromiseReturn(BackendResult result)
        {
            string cameFromScreen = "settings";

            if (pState.GetType() == typeof(NSSubscriptionDlg))
            {
                cameFromScreen = "subscription";
            }

            if (result == BackendResult.PURCHASE_COMPLETE)
            {
                int price = 0;
                item = metaDataModel.store.items[GSBackendKeys.ShopItem.SUBSCRIPTION_SHOP_TAG];
                if(item != null)
                {
                    price = item.currency1Cost;
                }

                HAnalytics.LogEvent(AnalyticsMonetizationEvent.Create("restore_ios_iap_completed").ST1("menu")
                                                                .ST2(cameFromScreen).Value(price));
            }
            else if (result == BackendResult.PURCHASE_FAILED)
            {
                HAnalytics.LogEvent(AnalyticsEvent.Create("restore_ios_iap_failed").ST1("menu")
                                                                .ST2(cameFromScreen));
            }
        }
    }
}
