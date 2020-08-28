/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public class TournamentLeaderboardMediator : Mediator
    {
        // View injection
        [Inject] public TournamentLeaderboardView view { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ITournamentsModel tournamentModel { get; set; }

        private LiveTournamentData liveTournament = null;
        private JoinedTournamentData joinedTournament = null;

        public override void OnRegister()
        {
            view.Init();

            // Button click handlers
            view.playerBarClickedSignal.AddListener(OnPlayerBarClicked);
            view.footer.enterButtonClickedSignal.AddListener(OnEnterButtonClicked);
            view.infoBar.rulesButtonClickedSignal.AddListener(OnRulesButtonClicked);
            view.infoBar.totalScoreButtonClickedSignal.AddListener(OnTotalScoreButtonClicked);
            view.infoBar.gameModeButtonClickedSignal.AddListener(OnGameModeButtonClicked);
            view.backSignal.AddListener(OnBackPressed);

        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.TOURNAMENT_LEADERBOARD_VIEW)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(GetTournamentLeaderboardSuccessSignal))]
        public void UpdateJoinedTournamentView(string tournamentId)
        {
            var joinedTournament = tournamentModel.GetJoinedTournament(tournamentId);
            if (joinedTournament != null)
            {
                view.UpdateView(joinedTournament);
            }
        }

        [ListensTo(typeof(FetchLiveTournamentRewardsSuccessSignal))]
        public void UpdateLiveTournamentView(string tournamentShortCode)
        {
            var openTournament = tournamentModel.GetOpenTournament(tournamentShortCode);
            if (openTournament != null)
            {
                view.UpdateView(openTournament);
            }
        }

        public void OnEnterButtonClicked()
        {
            TLUtils.LogUtil.Log("TournamentLeaderboardMediator::OnEnterButtonClicked()");
        }

        public void OnPlayerBarClicked(TournamentLeaderboardPlayerBar playerBar)
        {
            TLUtils.LogUtil.Log("TournamentLeaderboardMediator::OnPlayerBarClicked()");
        }

        public void OnRulesButtonClicked()
        {
            TLUtils.LogUtil.Log("TournamentLeaderboardMediator::OnRulesButtonClicked()");
        }

        public void OnTotalScoreButtonClicked()
        {
            TLUtils.LogUtil.Log("TournamentLeaderboardMediator::OnTotalScoreButtonClicked()");
        }

        public void OnGameModeButtonClicked()
        {
            TLUtils.LogUtil.Log("TournamentLeaderboardMediator::OnGameModeButtonClicked()");
        }

        private void OnBackPressed()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
