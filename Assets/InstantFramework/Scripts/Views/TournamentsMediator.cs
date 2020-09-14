/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public class TournamentsMediator : Mediator
    {
        // View injection
        [Inject] public TournamentsView view { get; set; }

        // Dispatch signals
        [Inject] public LeagueProfileStripSetOnClickSignal leagueProfileStripSetOnClickSignal { get; set; }
        [Inject] public UpdateTournamentLeaderboardPartialSignal updateTournamentLeaderboardPartialSignal { get; set; }
        [Inject] public FetchLiveTournamentRewardsSignal fetchLiveTournamentRewardsSignal { get; set; }
        [Inject] public GetTournamentLeaderboardSignal getJoinedTournamentLeaderboardSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public INotificationsModel notificationsModel { get; set; }

        private Signal onLeagueProfileStripClickedSignal = new Signal();

        public override void OnRegister()
        {
            view.Init();

            // Add button click signal to profile strip
            leagueProfileStripSetOnClickSignal.Dispatch(onLeagueProfileStripClickedSignal);
            onLeagueProfileStripClickedSignal.AddListener(OnLeagueProfileStripClicked);

            // Button click handlers
            view.liveItemClickedSignal.AddListener(OnUpcomingItemClicked);
            view.upcomingItemClickedSignal.AddListener(OnUpcomingItemClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.ARENA_VIEW)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.arena);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.ARENA_VIEW)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateTournamentsViewSignal))]
        public void UpdateView()
        {
            view.UpdateView();
        }

        public void OnUpcomingItemClicked(TournamentLiveItem item)
        {
            if (item.openTournamentData != null)
            {
                updateTournamentLeaderboardPartialSignal.Dispatch(item.openTournamentData.shortCode);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS);
                fetchLiveTournamentRewardsSignal.Dispatch(item.openTournamentData.shortCode);
            }
            else if (item.joinedTournamentData != null)
            {
                updateTournamentLeaderboardPartialSignal.Dispatch(item.joinedTournamentData.id);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS);
                getJoinedTournamentLeaderboardSignal.Dispatch(item.joinedTournamentData.id, true);
            }

            analyticsService.Event(AnalyticsEventId.tap_live_tournament);
        }

        public void OnUpcomingItemClicked(LiveTournamentData item)
        {
            TLUtils.LogUtil.Log("TournamentsMediator::OnUpcomingItemClicked()");
            analyticsService.Event(AnalyticsEventId.tap_notification);

            var tenMinInMilliseconds = 10 * 60 * 1000;

            if (((item.currentStartTimeUTCSeconds * 1000) - backendService.serverClock.currentTimestamp) > tenMinInMilliseconds)
            {
                var reminder = new Notification();
                reminder.title = $"{item.name} {view.localizationService.Get(LocalizationKey.NOTIFICATION_UPCOMING_TOURNAMENT_REMINDER_TITLE)}";
                reminder.body = view.localizationService.Get(LocalizationKey.NOTIFICATION_UPCOMING_TOURNAMENT_REMINDER_BODY);
                reminder.timestamp = (item.currentStartTimeUTCSeconds * 1000) - tenMinInMilliseconds;
                reminder.sender = $"{item.type}_upcoming";
                notificationsModel.RegisterNotification(reminder);
            }

            var notification = new Notification();
            notification.title = $"{item.name} {view.localizationService.Get(LocalizationKey.NOTIFICATION_UPCOMING_TOURNAMENT_STARTED_TITLE)}";
            notification.body = view.localizationService.Get(LocalizationKey.NOTIFICATION_UPCOMING_TOURNAMENT_STARTED_BODY);
            notification.timestamp = item.currentStartTimeUTCSeconds * 1000;
            notification.sender = $"{item.type}_upcoming";
            notificationsModel.RegisterNotification(notification);
        }

        public void OnLeagueProfileStripClicked()
        {
            TLUtils.LogUtil.Log("TournamentsMediator::OnLeagueProfileStripClicked()");
        }
    }
}
