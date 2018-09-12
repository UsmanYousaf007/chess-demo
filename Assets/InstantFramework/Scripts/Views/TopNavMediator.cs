﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class TopNavMediator : Mediator
    {
        // View injection
        [Inject] public TopNavView view { get; set; }

        // Dispatch signals
        [Inject] public ShareAppSignal shareAppSignal { get; set; }
        [Inject] public LoadBuckPacksSignal loadBuckPacksSignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.shareAppButtonClickedSignal.AddListener(OnShareAppButtonClicked);
            view.addBucksButtonClickedSignal.AddListener(OnAddBucksButtonClicked);
            view.removeAdsButtonClickedSignal.AddListener(OnRemoveAdsButtonClicked);
        }

        public override void OnRemove()
        {
            view.shareAppButtonClickedSignal.RemoveAllListeners();
            view.addBucksButtonClickedSignal.RemoveAllListeners();
        }

        private void OnShareAppButtonClicked()
        {
            shareAppSignal.Dispatch();
        }

        private void OnAddBucksButtonClicked()
        {
            loadBuckPacksSignal.Dispatch();
        }

        private void OnRemoveAdsButtonClicked()
        {
            purchaseStoreItemSignal.Dispatch(GSBackendKeys.SHOP_ITEM_FEATURE_REMOVE_ADS, true);
        }

        [ListensTo(typeof(UpdatePlayerBucksSignal))]
        public void OnUpdatePlayerBucksDisplay(long playerBucks)
        {
            view.UpdatePlayerBucks(playerBucks);
        }

        [ListensTo(typeof(UpdateRemoveAdsSignal))]
        public void OnUpdateRemoveAdsDisplay(string freePeriod, bool isRemoved)
        {
            view.UpdateRemoveAds(freePeriod, isRemoved);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnStoreAvailable(isAvailable);
        }
    }
}
