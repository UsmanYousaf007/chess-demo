/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:53:20 UTC+05:00
/// 
/// @description
/// [add_description_here]

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

        public override void OnRegister()
        {
            view.Init();

            view.shareAppButtonClickedSignal.AddListener(OnShareAppButtonClicked);
            view.addBucksButtonClickedSignal.AddListener(OnAddBucksButtonClicked);
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

        [ListensTo(typeof(UpdatePlayerBucksSignal))]
        public void OnUpdatePlayerBucksDisplay(long playerBucks)
        {
            view.UpdatePlayerBucks(playerBucks);
        }
    }
}
