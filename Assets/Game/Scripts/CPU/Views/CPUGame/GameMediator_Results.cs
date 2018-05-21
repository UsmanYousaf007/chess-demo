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

using TurboLabz.InstantFramework;
using TurboLabz.Chess;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess 
{
    public partial class GameMediator
    {
        // Dispatch signal
        [Inject] public LoadCPUGameSignal loadGameSignal { get; set; }
        [Inject] public LoadStatsSignal loadStatsSignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }
        [Inject] public EnterPlaybackSignal enterPlaybackSignal { get; set; }

        public void OnRegisterResults()
        {
            view.InitResults();
            view.resultsExitButtonClickedSignal.AddListener(OnResultsExitButtonClicked);
            view.showAdButtonClickedSignal.AddListener(OnShowAdButtonClicked);
            view.enterPlaybackSignal.AddListener(OnEnterPlayback);
            view.resultsDialogButtonClickedSignal.AddListener(OnResultsDialogButtonClicked);
			view.resultsStatsButtonClickedSignal.AddListener(OnResultsStatsButtonClicked);
        }

        public void OnRemoveResults()
        {
            view.CleanupResults();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.RESULTS_DLG) 
            {
                view.ShowResultsDialog();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.RESULTS_DLG)
            {
                view.HideResultsDialog();
            }
        }

        [ListensTo(typeof(UpdateResultDialogSignal))]
		public void OnUpdateResults(GameEndReason gameEndReason, bool playerWins, int statResult)
        {
            view.UpdateResultsDialog(gameEndReason, playerWins, statResult);
        }


        [ListensTo(typeof(EnableResultsDialogButtonSignal))]
        public void OnEnableResultsDialogButton()
        {
            view.EnableResultsDialogButton();
        }

        private void OnResultsExitButtonClicked()
        {
            loadGameSignal.Dispatch();
        }

        private void OnShowAdButtonClicked()
        {
            showAdSignal.Dispatch();
        }

        private void OnEnterPlayback()
        {
            enterPlaybackSignal.Dispatch();
        }

        private void OnResultsDialogButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_RESULTS_DLG);
        }

		private void OnResultsStatsButtonClicked()
		{
			loadStatsSignal.Dispatch();
		}
    }
}
