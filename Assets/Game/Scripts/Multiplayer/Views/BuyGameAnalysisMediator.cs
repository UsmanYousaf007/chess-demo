/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;
using TurboLabz.Chess;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class BuyGameAnalysisMediator : Mediator
    {
        // View injection
        [Inject] public BuyGameAnalysisView view { get; set; }

        //Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public GetFullAnalysisSignal getFullAnalysisSignal { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }


        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.BUY_GAME_ANALYSIS_DLG)
            {
                view.Show();
                view.notEnoughGemsSignal.AddListener(OnNotEnoughGems);
                view.closeDlgSignal.AddListener(OnDialogClosedSignal);
                view.fullAnalysisButtonClickedSignal.AddListener(OnFullAnallysisButtonClicked);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.BUY_GAME_ANALYSIS_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateGetGameAnalysisDlgSignal))]
        public void OnUpdateView(MatchAnalysis matchAnalysis, StoreItem storeItem, bool availableForFree)
        {
            view.UpdateView(matchAnalysis, storeItem, availableForFree);
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            if (view.isActiveAndEnabled)
            {
                view.SetupPrice();
            }
        }

        private void OnFullAnallysisButtonClicked()
        {
            getFullAnalysisSignal.Dispatch();
        }

        private void OnDialogClosedSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
        }

        private void OnNotEnoughGems()
        {
            SpotPurchaseMediator.analyticsContext = "game_analysis";
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        }
    }
}
