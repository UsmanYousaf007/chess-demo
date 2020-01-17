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

        public override void OnRegister()
        {
            view.Init();

            view.addBucksButtonClickedSignal.AddListener(OnAddBucksButtonClicked);
        }

        public override void OnRemove()
        {
            view.addBucksButtonClickedSignal.RemoveAllListeners();
        }

        private void OnAddBucksButtonClicked()
        {

        }

        [ListensTo(typeof(UpdateRemoveAdsSignal))]
        public void OnUpdateRemoveAdsDisplay(string freePeriod, bool isRemoved)
        {
            view.UpdateRemoveAds(freePeriod, isRemoved);
        }
    }
}
