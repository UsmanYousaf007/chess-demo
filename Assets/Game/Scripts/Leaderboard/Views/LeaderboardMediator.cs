using System;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class LeaderboardMediator : Mediator
    {
        //View Injection
        [Inject] public LeaderboardView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public ISchedulerService schedulerService { get; set; }

        // Models
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public ILeaderboardModel leaderboardModel { get; set; }

        // Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public GetAllStarLeaderboardSignal getAllStarLeaderboardSignal { get; set; }
        [Inject] public GetProfilePictureSignal getProfilePictureSignal { get; set; }
        [Inject] public UpdateTournamentLeaderboardPartialSignal updateTournamentLeaderboardPartialSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getJoinedTournamentLeaderboardSignal { get; set; }
        [Inject] public FetchLiveTournamentRewardsSignal fetchLiveTournamentRewardsSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();

            view.backSignal.AddListener(OnBackPressed);
            view.loadPictureSignal.AddListener(OnLoadPicture);
            view.schedulerSubscription.AddListener(OnSchedulerSubscriptionToggle);

            view.serverClock = backendService.serverClock;
        }

        private void OnLoadPicture(GetProfilePictureVO vo)
        {
            getProfilePictureSignal.Dispatch(vo);
        }

        [ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateView()
        {
            if (view.gameObject.activeInHierarchy)
            {
                view.UpdateView(tournamentsModel.GetJoinedTournament());
            }
        }

        [ListensTo(typeof(ProfilePictureLoadedSignal))]
        public void OnPictureLoaded(string playerId, Sprite picture)
        {
            if (view.isActiveAndEnabled)
            {
                view.UpdatePicture(playerId, picture);
            }
        }


        [ListensTo(typeof(UpdateAllStarLeaderboardSignal))]
        public void OnUpdateAllStarLeaderboard()
        {
            if (view.gameObject.activeInHierarchy)
            {
                view.UpdateView(leaderboardModel.allStarLeaderboardEntries);
            }
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LEADERBOARD_VIEW)
            {
                view.UpdateLeague();
                view.Show(tournamentsModel.GetJoinedTournament());
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

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnDataAvailable(isAvailable);
        }

        private void OnBackPressed()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnSchedulerSubscriptionToggle(Action callback, bool subscribe)
        {
            if (subscribe)
            {
                schedulerService.Subscribe(callback);
            }
            else
            {
                schedulerService.UnSubscribe(callback);
            }
        }

        [ListensTo(typeof(ReconnectResetStateSignal))]
        public void ReconnectResetState()
        {
            if (view.isActiveAndEnabled)
            {
                JoinedTournamentData joinedTournament = tournamentsModel.GetJoinedTournament();
                LiveTournamentData openTournament = tournamentsModel.GetOpenTournament();

                if (joinedTournament != null)
                {
                    updateTournamentLeaderboardPartialSignal.Dispatch(joinedTournament.id);
                    getJoinedTournamentLeaderboardSignal.Dispatch(joinedTournament.id, true);
                }
                else if (openTournament != null)
                {
                    updateTournamentLeaderboardPartialSignal.Dispatch(openTournament.shortCode);
                    fetchLiveTournamentRewardsSignal.Dispatch(openTournament.shortCode);
                }
            }
        }
    }
}
