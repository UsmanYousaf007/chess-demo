using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class ChampionshipNewRankDlgMediator : Mediator
    {
        //View Injection
        [Inject] public ChampionshipNewRankDlgView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getChampionshipTournamentLeaderboardSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.continueBtnClickedSignal.AddListener(OnContinuePressed);
        }

        [ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateView()
        {
            if (view.gameObject.activeInHierarchy)
            {
                view.UpdateView(playerModel.id, tournamentsModel.GetJoinedTournament());
            }
        }

        //[ListensTo(typeof(ResetTournamentsViewSignal))]
        //public void ResetView()
        //{
        //    view.ResetView();
        //}

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_NEW_RANK_DLG)
            {
                JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament();
                if (joinedTournament != null && joinedTournament.entries.Count > 0)
                {
                    view.UpdateView(playerModel.id, joinedTournament);
                }
                else
                {
                    getChampionshipTournamentLeaderboardSignal.Dispatch(joinedTournament.id, false);
                }

                view.UpdateLeagueTitle(playerModel, tournamentsModel);
                view.Show();
                //analyticsService.ScreenVisit(AnalyticsScreen.inventory);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CHAMPIONSHIP_NEW_RANK_DLG)
            {
                view.Hide();
                view.ResetView();
            }
        }

        private void OnContinuePressed()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
