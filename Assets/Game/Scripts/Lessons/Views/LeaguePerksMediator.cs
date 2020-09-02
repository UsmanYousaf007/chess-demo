/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
    public class LeaguePerksMediator : Mediator
    {
        // View injection
        [Inject] public LeaguePerksView view { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        
        //Analytics Service
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.OnBackButtonClickedSignal.AddListener(OnBackButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LEAGUE_PERKS_VIEW)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.LEAGUE_PERKS_VIEW)
            {
                view.Hide();
            }
        }

        private void OnBackButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_ARENA);
        }

        [ListensTo(typeof(UpdateLeagueProfileSignal))]
        public void UpdateLeague(string leagueID)
        {
            view.UpdateLeague(leagueID);
        }
    }
}
