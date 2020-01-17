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

        [Inject] public SettingsButtonClickedSignal settingsButtonClickedSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.addBucksButtonClickedSignal.AddListener(OnAddBucksButtonClicked);
            view.settingsButtonClickedSignal.AddListener(OnSettingsButtonClicked);

        }

        public override void OnRemove()
        {
            view.addBucksButtonClickedSignal.RemoveAllListeners();
        }

        private void OnAddBucksButtonClicked()
        {

        }

        private void OnSettingsButtonClicked()
        {
            Debug.Log("Dispatch Settings Button Clicked: Top Nav Mediator");
            settingsButtonClickedSignal.Dispatch();
        }

        [ListensTo(typeof(UpdateRemoveAdsSignal))]
        public void OnUpdateRemoveAdsDisplay(string freePeriod, bool isRemoved)
        {
            view.UpdateRemoveAds(freePeriod, isRemoved);
        }
    }
}
