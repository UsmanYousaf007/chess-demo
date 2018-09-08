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
using TurboLabz.InstantGame;

namespace TurboLabz.Multiplayer 
{
    public partial class GameMediator
    {
        // Dispatch signal
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }
        [Inject] public ShowAdSignal showAdSignal { get; set; }

        public void OnRegisterResults()
        {
            view.InitResults();
            view.backToLobbySignal.AddListener(OnBackToLobby);
            view.backToFriendsSignal.AddListener(OnBackToFriends);
            view.resultsDialogButtonClickedSignal.AddListener(OnResultsDialogButtonClicked);
        }

        public void OnRemoveResults()
        {
            view.CleanupResults();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_RESULTS_DLG) 
            {
                view.ShowResultsDialog();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideResultsView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.MULTIPLAYER_RESULTS_DLG)
            {
                view.HideResultsDialog();
            }
        }

        [ListensTo(typeof(AppEventSignal))]
        public void OnAppEvent(AppEvent evt)
        {
            if (evt == AppEvent.ESCAPED)
            {
                view.ExitPlaybackMode();
            }
        }
            
        [ListensTo(typeof(UpdateResultDialogSignal))]
        public void OnUpdateResults(ResultsVO vo)
        {
            view.UpdateResultsDialog(vo);
        }

        private void OnBackToLobby()
        {
            loadLobbySignal.Dispatch();
            showAdSignal.Dispatch(false);
        }

        private void OnResultsDialogButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG);
        }
    }
}
