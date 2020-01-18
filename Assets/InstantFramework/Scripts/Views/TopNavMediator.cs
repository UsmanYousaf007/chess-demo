/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class TopNavMediator : Mediator
    {
        // View injection
        [Inject] public TopNavView view { get; set; }

        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.settingsButtonClickedSignal.AddListener(OnSettingsButtonClicked);
            view.selectThemeClickedSignal.AddListener(OnSelectThemeClicked);
        }

        public override void OnRemove()
        {
            view.settingsButtonClickedSignal.RemoveAllListeners();
            view.selectThemeClickedSignal.RemoveAllListeners();
        }

        private void OnSettingsButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SETTINGS);
        }

        [ListensTo(typeof(UpdateRemoveAdsSignal))]
        public void OnUpdateRemoveAdsDisplay(string freePeriod, bool isRemoved)
        {
            view.UpdateRemoveAds(freePeriod, isRemoved);
        }

        private void OnSelectThemeClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_THEME_SELECTION_DLG);
        }

        [ListensTo(typeof(RewardUnlockedSignal))]
        public void OnRewardUnlocked(string key, int quantity)
        {
            view.OnRewardUnlocked(key, quantity);
        }
    }
}
