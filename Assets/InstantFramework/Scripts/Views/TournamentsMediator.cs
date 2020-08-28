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
        [Inject] public FetchLiveTournamentRewardsSignal fetchLiveTournamentRewardsSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

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
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_TOURNAMENT_LEADERBOARDS);
                fetchLiveTournamentRewardsSignal.Dispatch(item.openTournamentData.shortCode);
            }
        }

        public void OnUpcomingItemClicked(TournamentUpcomingItem item)
        {
            TLUtils.LogUtil.Log("TournamentsMediator::OnUpcomingItemClicked()");
        }

        public void OnLeagueProfileStripClicked()
        {
            TLUtils.LogUtil.Log("TournamentsMediator::OnLeagueProfileStripClicked()");
        }
    }
}
