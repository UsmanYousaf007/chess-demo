/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:03:30 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Gamebet;
using TurboLabz.Chess;
using TurboLabz.Common;

namespace TurboLabz.CPUChess 
{
    public partial class GameMediator
    {
        // Dispatch signals
        [Inject] public LoadCPUMenuSignal loadCPUMenuSignal { get; set; }

        public void OnRegisterResults()
        {
            view.InitResults();
            view.resultsExitButtonClickedSignal.AddListener(OnResultsExitButtonClicked);
            view.statsButtonClickedSignal.AddListener(OnStatsButtonClickedSignal);
        }

        public void OnRemoveResults()
        {
            view.CleanupResults();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_PLAY_RESULTS) 
            {
                view.ShowResultsDialog();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_PLAY_RESULTS)
            {
                view.HideResultsDialog();
            }
        }

        [ListensTo(typeof(UpdateResultDialogSignal))]
        public void OnUpdateResults(GameEndReason gameEndReason, bool playerWins)
        {
            view.UpdateResultsDialog(gameEndReason, playerWins);
        }

        private void OnResultsExitButtonClicked()
        {
            loadCPUMenuSignal.Dispatch();
        }

        private void OnStatsButtonClickedSignal()
        {

        }
    }
}
