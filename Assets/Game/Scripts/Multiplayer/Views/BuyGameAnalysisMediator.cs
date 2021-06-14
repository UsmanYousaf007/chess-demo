/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using strange.extensions.mediation.impl;
using System;


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
        [Inject] public ShowRewardedAdSignal showRewardedAdSignal { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }
        [Inject] public ISchedulerService schedulerService { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.serverClock = backendService.serverClock;
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
                view.schedulerSubscription.AddListener(OnSchedulerSubscriptionToggle);
                view.watchVideoSignal.AddListener(OnWatchVideoSignal);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.BUY_GAME_ANALYSIS_DLG)
            {
                view.Hide();
                OnSchedulerSubscriptionToggle(false);
            }
        }

        [ListensTo(typeof(UpdateGetGameAnalysisDlgSignal))]
        public void OnUpdateView(BuyGameAnalysisVO vo)
        {
            view.UpdateView(vo);
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

        private void OnSchedulerSubscriptionToggle(bool subscribe)
        {
            if (subscribe)
            {
                schedulerService.Subscribe(view.SchedulerCallBack);
            }
            else
            {
                schedulerService.UnSubscribe(view.SchedulerCallBack);
            }
        }

        private void OnWatchVideoSignal()
        {
            showRewardedAdSignal.Dispatch(AdPlacements.Rewarded_analysis);
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardedVideoResult(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled)
            {
                if (result == AdsResult.NOT_AVAILABLE || result == AdsResult.FAILED)
                {
                    if (adPlacement == AdPlacements.Rewarded_analysis)
                    {
                        view.EnableRVNotAvailableTooltip();
                    }
                }
            }
        }
    }
}
