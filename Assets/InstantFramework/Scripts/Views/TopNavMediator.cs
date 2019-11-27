/// @license Propriety <http://license.url>
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
        [Inject] public LoadStoreSignal loadStoreSignal { get; set; }
        [Inject] public ShowStoreTabSignal showStoreTabSignal { get; set; }
        [Inject] public ISupportService supportService { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.addBucksButtonClickedSignal.AddListener(OnAddBucksButtonClicked);
            view.supportButtonClickedSignal.AddListener(OnSupportButtonClicked);
        }

        public override void OnRemove()
        {
            view.addBucksButtonClickedSignal.RemoveAllListeners();
        }

        private void OnAddBucksButtonClicked()
        {
            loadStoreSignal.Dispatch();
            showStoreTabSignal.Dispatch(StoreView.StoreTabs.COINS);
        }

        private void OnSupportButtonClicked()
        {
            supportService.ShowFAQ();
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
        public void OnStoreAvailable(bool isAvailable, StoreVO storeVO)
        {
            view.OnStoreAvailable(isAvailable);
        }
    }
}
