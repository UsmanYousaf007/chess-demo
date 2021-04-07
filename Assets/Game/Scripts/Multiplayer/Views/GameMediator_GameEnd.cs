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
                view.OnParentHideAdBanner();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_RESULTS_DLG)
            {
                view.HideGameEndDialog();
            }
        }

        [ListensTo(typeof(UpdateResultDialogSignal))]
        public void OnUpdateResults(ResultsVO vo)
        {
            view.UpdateResultsDialog(vo);
            view.UpdateRewardsDialog(vo);
            view.UpdateEndGame();
        }
    }
}
