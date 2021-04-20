/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;
using UnityEngine;

namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        // Dispatch Signals
        [Inject] public UpdateNewRankChampionshipDlgViewSignal updateNewRankChampionshipDlgViewSignal { get; set; }

        public void OnRegisterGameEnd()
        {
            view.showWeeklyChampionshipResultsSignal.AddListener(OnShowWeeklyChampionshipResultsSignal);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_RESULTS_DLG)
            {
                if (view.challengeSentDialog.activeSelf)
                {
                    view.HideChallengeSent();
                }
                view.FlashClocks(false);
                view.ShowEndGame();
                view.ShowResultsDialog();
                view.OnParentHideAdBanner();
            }
            else if(viewId == NavigatorViewId.MULTIPLAYER_REWARDS_DLG)
            {
                if (view.challengeSentDialog.activeSelf)
                {
                    view.HideChallengeSent();
                }
                view.FlashClocks(false);
                view.ShowEndGame();
                view.StartEndAnimationSequence();
                view.OnParentHideAdBanner();
            }
            else if (viewId == NavigatorViewId.GAME_ANALYZING_DLG)
            {
                view.ShowAnalyzingGame();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_RESULTS_DLG)
            {
                view.HideGameEndDialog();
            }
            else if (viewId == NavigatorViewId.MULTIPLAYER_REWARDS_DLG)
            {
                view.HideGameEndDialog();
                view.HideRewardsDialog();
            }
            else if (viewId == NavigatorViewId.GAME_ANALYZING_DLG)
            {
                view.HideGameAnalyzingDlg();
            }
        }

        [ListensTo(typeof(UpdateResultDialogSignal))]
        public void OnUpdateResults(ResultsVO vo)
        {
            view.UpdateResultsDialog(vo);
            view.UpdateRewardsDialog(vo);
            view.UpdateEndGame();
        }

        private void OnShowWeeklyChampionshipResultsSignal(string challengeId, bool playerWins, float TRANSITION_DURATION)
        {
            updateNewRankChampionshipDlgViewSignal.Dispatch(challengeId, playerWins, TRANSITION_DURATION);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CHAMPIONSHIP_NEW_RANK_DLG);
        }
    }
}
