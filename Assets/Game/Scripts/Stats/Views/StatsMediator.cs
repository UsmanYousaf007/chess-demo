/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-03 16:10:58 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine;

using strange.extensions.mediation.impl;

using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
    public class StatsMediator : Mediator
    {
        // Dispatch signals
        [Inject] public RestorePurchasesSignal restorePurchasesSignal { get; set; }
        [Inject] public LoadLobbySignal loadLobbySignal { get; set; }

        // View injection
        [Inject] public StatsView view { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.restorePurchasesSignal.AddListener(OnRestorePurchases);
            view.backButton.onClick.AddListener(OnBackButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.STATS) 
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.profile);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.STATS)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateStatsSignal))]
        public void OnUpdateStats(StatsVO vo)
        {
            view.UpdateView(vo);
        }

        void OnRestorePurchases()
        {
            restorePurchasesSignal.Dispatch();
        }

        void OnBackButtonClicked()
        {
            loadLobbySignal.Dispatch();
        }
    }
}
