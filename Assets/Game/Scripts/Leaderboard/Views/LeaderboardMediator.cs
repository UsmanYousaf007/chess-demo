using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;

namespace TurboLabz.InstantFramework
{
    public class LeaderboardMediator : Mediator
    {
        //View Injection
        [Inject] public LeaderboardView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public ILeaderboardModel leaderboardModel { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public GetAllStarLeaderboardSignal getAllStarLeaderboardSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.backSignal.AddListener(OnBackPressed);
        }

        [ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateView()
        {
            if (view.gameObject.activeInHierarchy)
            {
                view.UpdateView(tournamentsModel.GetJoinedTournament());
            }
        }

        //[ListensTo(typeof(ResetTournamentsViewSignal))]
        //public void ResetView()
        //{
        //    view.ResetChampionshipView();
        //}

        [ListensTo(typeof(UpdateAllStarLeaderboardSignal))]
        public void OnUpdateAllStarLeaderboard()
        {
            view.UpdateView(leaderboardModel.allStarLeaderboardEntries);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LEADERBOARD_VIEW)
            {
                JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament();
                if (joinedTournament != null && joinedTournament.entries.Count > 0)
                {
                    view.UpdateView(tournamentsModel.GetJoinedTournament());
                }
                else
                {
                    getAllStarLeaderboardSignal.Dispatch();
                }

                view.UpdateLeague();
                view.Show();
                //analyticsService.ScreenVisit(AnalyticsScreen.inventory);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {                                
            if (viewId == NavigatorViewId.LEADERBOARD_VIEW)
            {
                view.Hide();
                view.ResetChampionshipView();
            }
        }

        //[ListensTo(typeof(UpdateProfileSignal))]
        //public void OnUpdateProfile(ProfileVO vo)
        //{
        //    view.UpdateView();
        //}

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnDataAvailable(isAvailable);
        }

        private void OnBackPressed()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
