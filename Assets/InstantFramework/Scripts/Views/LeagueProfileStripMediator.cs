/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using TurboLabz.TLUtils;
using System.Collections.Generic;
using TurboLabz.InstantGame;
using System;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public class LeagueProfileStripMediator : Mediator
    {
        // View injection
        [Inject] public LeagueProfileStripView view { get; set; }

        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IHAnalyticsService hAnalyticsService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.leagueProfileClickedSignal.AddListener(OnLeagueProfileClicked);
        }

        [ListensTo(typeof(UpdateLeagueProfileStripSignal))]
        public void OnUpdateProfile(LeagueProfileStripVO vo)
        {
            view.UpdateView(vo);
        }

        [ListensTo(typeof(PlayerModelUpdatedSignal))]
        public void OnPlayerModelUpdated(IPlayerModel playerModel)
        {
            view.UpdateLeague(playerModel.league);
            view.UpdateTrophies(playerModel.trophies);
        }

        [ListensTo(typeof(LeagueProfileStripSetOnClickSignal))]
        public void OnSetStripClickedSignal(Signal signal)
        {
            view.SetStripClickedSignal(signal);
        }

        public void OnLeagueProfileClicked()
        {
            analyticsService.Event(AnalyticsEventId.tap_tier_info);
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_LEAGUE_PERKS_VIEW);
        }
    }
}
