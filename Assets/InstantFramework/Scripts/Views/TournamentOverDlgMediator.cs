/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public class TournamentOverDlgMediator : Mediator
    {
        [Inject] public TournamentOverDlgView view { get; set; }

        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public TournamentOverDialogueClosedSignal tournamentOverDialogueClosedSignal { get; set; }

        // Services
        [Inject] public IAudioService audioService { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.buttonClickedSignal.AddListener(OnButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShow(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.TOURNAMENT_OVER_DLG)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHide(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.TOURNAMENT_OVER_DLG)
            {
                view.Hide();
            }
        }

        public void OnButtonClicked()
        {
            audioService.PlayStandardClick();

            view.Hide();

            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS);

            tournamentOverDialogueClosedSignal.Dispatch();
        }
    }
}