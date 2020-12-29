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

        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateView()
        {
            view.UpdateView(tournamentsModel.GetJoinedTournament());
        }

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
            }
        }

        [ListensTo(typeof(UpdateProfileSignal))]
        public void OnUpdateProfile(ProfileVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnDataAvailable(isAvailable);
        }

        public void OnLeaderboardUpdated()
        {
            
        }
    }
}
