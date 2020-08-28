/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class ChestContentDialogMediator : Mediator
    {
        // Dispatch signals

        // View injection
        [Inject] public ChestInfoDialogView view { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IAudioService audioService { get; set; }


        public override void OnRegister()
        {
            view.Init();

            view.closeSignal.AddListener(OnClose);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHEST_INFO_DLG) 
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHEST_INFO_DLG)
            {
                view.Hide();
            }
        }

        private void OnClose()
        {
            audioService.PlayStandardClick();
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        [ListensTo(typeof(UpdateChestInfoDlgViewSignal))]
        private void UpdateView(TournamentReward reward)
        {
            view.UpdateView(reward);
        }
    }
}
