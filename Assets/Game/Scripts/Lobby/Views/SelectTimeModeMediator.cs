/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using System;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class SelectTimeModeMediator : Mediator
    {
        // View injection
        [Inject] public SelectTimeModeView view { get; set; }

        //Dispatch signals
        [Inject] public FindMatchSignal findMatchSignal { get; set; }
        [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ShowRewardedAdSignal rewardedAdSignal { get; set; }
        //Listerners
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public ITournamentsModel tournamentsModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }
        //Service
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPreGameAdsService preGameAdsService { get; set; }
        [Inject] public ISchedulerService schedulerService { get; set; }

        private MatchInfoVO matchInfoVO;
        private long betValue;
        private bool rewardedVideoShown;
        public override void OnRegister()
        {
            view.Init();
            view.playMultiplayerButtonClickedSignal.AddListener(OnPlayMatch);
            view.powerModeButtonClickedSignal.AddListener(OnPowerModeButtonClicked);
            view.notEnoughCoinsSignal.AddListener(OnNotEnoughCoinsSignal);
            view.notEnoughGemsSignal.AddListener(OnNotEnoughGemsSignal);
            view.closeButtonSignal.AddListener(OnCloseSignal);
            view.showRewardedAdSignal.AddListener(OnPlayRewardedVideoClicked);
            view.schedulerSubscription.AddListener(OnSchedulerSubscriptionToggle);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SELECT_TIME_MODE)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SELECT_TIME_MODE)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateTimeSelectDlgSignal))]
        public void UpdateView(SelectTimeDlgVO selectTimeDlgVO)
        {
            this.betValue = selectTimeDlgVO.bet;
            view.UpdateView(selectTimeDlgVO);
        }

        private void OnPlayMatch(string actionCode, bool isPowerMode)
        {
            matchInfoVO = new MatchInfoVO();
            matchInfoVO.actionCode = actionCode;
            matchInfoVO.betValue = betValue;
            matchInfoVO.powerMode = isPowerMode;


            if (!this.rewardedVideoShown)
            {
                preGameAdsService.ShowPreGameAd(matchInfoVO.actionCode).Then(() =>
                {
                    CreateVGoodsTransaction();
                });
            }
            else
            {
                CreateVGoodsTransaction();
            }
        }

        private void CreateVGoodsTransaction()
        {
            var transactionVO = new VirtualGoodsTransactionVO();
            transactionVO.consumeItemShortCode = GSBackendKeys.PlayerDetails.COINS;
            transactionVO.consumeQuantity = (int)betValue;
            virtualGoodsTransactionResultSignal.AddOnce(OnTransactionResult);
            virtualGoodsTransactionSignal.Dispatch(transactionVO);
        }

        private void OnTransactionResult(BackendResult result)
        {
            if (result == BackendResult.SUCCESS)
            {
                FindMatchAction.Random(findMatchSignal, matchInfoVO, tournamentsModel.GetJoinedTournament().id);
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.COINS, (int)betValue, "championship_coins", "bet_placed");
            }
        }

        private void OnPowerModeButtonClicked()
        {
            purchaseStoreItemSignal.Dispatch(view.shortCode, true);
        }

        private void OnNotEnoughCoinsSignal()
        {

        }

        private void OnNotEnoughGemsSignal()
        {
            SpotPurchaseMediator.analyticsContext = "power_mode";
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
        }

        private void OnCloseSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnItemPurchased(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.PURCHASE_SUCCESS && item.key.Equals(view.shortCode) && view.isActiveAndEnabled)
            {
                view.OnEnablePowerMode();
                analyticsService.Event(AnalyticsEventId.gems_used, AnalyticsContext.power_mode);
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "booster_used", AnalyticsContext.power_mode.ToString());
            }
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardClaimed(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.Rewarded_powerplay )
            {
                if ((result == AdsResult.FINISHED || result == AdsResult.SKIPPED))
                {
                    this.rewardedVideoShown = true;
    
                }

                else if (result == AdsResult.NOT_AVAILABLE)
                {
                    view.SetupVideoAvailabilityTooltip(true);
                }
            }
            
        }

        [ListensTo(typeof(PowerPlayRewardClaimedSignal))]
        public void OnPowerPlayRewardClaimed()
        {
            if (view.isActiveAndEnabled)
            {
                view.OnEnablePowerMode();
                view.StartTimer(playerModel.rvUnlockTimestamp);
            }
        }

        private void OnPlayRewardedVideoClicked(AdPlacements adPlacements)
        {
            rewardedAdSignal.Dispatch(adPlacements);
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
    }
}
