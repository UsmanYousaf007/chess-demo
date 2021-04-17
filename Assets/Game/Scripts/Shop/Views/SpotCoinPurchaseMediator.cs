using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class SpotCoinPurchaseMediator : Mediator
    {
        //View Injection
        [Inject] public SpotCoinPurchaseView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IAdsService adsService { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public LoadTimeSelectDlgSignal loadTimeSelectDlgSignal { get; set; }
        [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }
        [Inject] public ShowRewardedAdSignal showRewardedAdSignal { get; set; }
        [Inject] public LoadCareerCardSignal loadCareerCardSignal { get; set; }
        [Inject] public UpdateRewardDlgV2ViewSignal updateRewardDlgViewSignal { get; set; }
        [Inject] public SpotCoinsPurchaseDlgClosedSignal spotCoinsPurchaseDlgClosedSignal { get; set; }
        [Inject] public RewardSequenceV2ClosedSignal rewardSequenceV2ClosedSignal { get; set; }

        //Models
        [Inject] public IPlayerModel playerModel { get; set; }

        private long betValue;
        private AdPlacements adPlacement;
        private bool adView = false;

        public override void OnRegister()
        {
            view.Init();
            view.closeDlgSignal.AddListener(OnCloseDlgSignal);
            view.buyButtonClickedSignal.AddListener(OnBuySignal);
            view.watchAdButtonClickedSignal.AddListener(OnWatchAdSignal);
            view.closeDlgWithAnalyticSignal.AddListener(OnCloseDlgWithAnalytic);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_COIN_PURCHASE_DLG)
            {
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.spot_coin_purchase_dlg);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_COIN_PURCHASE_DLG)
            {
                view.Hide();
                spotCoinsPurchaseDlgClosedSignal.Dispatch();
            }
        }
        
        [ListensTo(typeof(UpdateSpotCoinsPurchaseDlgSignal))]
        public void OnUpdateView(long betValue, List<string> packs)
        {
            this.betValue = betValue;
            view.UpdateView(packs);
            adView = false;
        }

        [ListensTo(typeof(UpdateSpotCoinsWatchAdDlgSignal))]
        public void OnUpdateAdView(long betValue, StoreItem storeItem, AdPlacements adPlacement)
        {
            this.betValue = betValue;
            this.adPlacement = adPlacement;
            view.UpdateAdDlg(storeItem, adsService.IsPersonalisedAdDlgShown());
            adView = true;
            analyticsService.Event(AnalyticsEventId.drop_off_area_shown, AnalyticsContext.out_of_coins_pop_up);
        }

        [ListensTo(typeof(VirtualGoodBoughtSignal))]
        public void OnCoinsPurchased(VirtualGoodsTransactionVO transactionVO)
        {
            if (view.isActiveAndEnabled && transactionVO.buyItemShortCode.Equals(GSBackendKeys.PlayerDetails.COINS))
            {
                OnCloseDlgSignal();

                if (betValue > 0)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SELECT_TIME_MODE);
                    loadTimeSelectDlgSignal.Dispatch(betValue);
                }
                else
                {
                    loadCareerCardSignal.Dispatch();
                }

                if (transactionVO.buyQuantity > 0)
                {
                    var state = adView ? 1 : 2;
                    analyticsService.Event(AnalyticsEventId.coin_popup_purchase, AnalyticsParameter.context, $"{transactionVO.buyQuantity}_coins_pack_state_{state}");
                    analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.COINS, transactionVO.buyQuantity, "spot_purchase", $"coins_{transactionVO.buyQuantity}_state_{state}");
                    analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, transactionVO.consumeQuantity, "spot_purchase", $"coins_{transactionVO.buyQuantity}_state_{state}");
                }
            }
        }

        private void OnCloseDlgSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnCloseDlgWithAnalytic()
        {
            OnCloseDlgSignal();
            analyticsService.Event(AnalyticsEventId.drop_off_area_interaction, AnalyticsContext.out_of_coins_pop_up_closed);
        }

        private void OnWatchAdSignal()
        {
            showRewardedAdSignal.Dispatch(adPlacement);
            analyticsService.Event(AnalyticsEventId.drop_off_area_interaction, AnalyticsContext.out_of_coins_pop_up);
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardClaimed(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == this.adPlacement)
            {
                switch (result)
                {
                    case AdsResult.FINISHED:
                        var rewardDlgVO = new RewardDlgV2VO();
                        rewardDlgVO.Rewards.Add(new RewardDlgV2VO.Reward(GSBackendKeys.PlayerDetails.COINS, 2000));
                        rewardDlgVO.RVWatched = true;
                        navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_REWARD_DLG_V2);
                        updateRewardDlgViewSignal.Dispatch(rewardDlgVO);
                        rewardSequenceV2ClosedSignal.AddOnce(() => StartCoroutine(DispatchCloseSignalAsync()));
                        break;

                    case AdsResult.NOT_AVAILABLE:
                        view.toolTip.SetActive(true);
                        break;
                }
            }
        }

        private void OnBuySignal(StoreItem storeItem)
        {
            if (playerModel.gems >= storeItem.currency3Payout)
            {
                var vo = new VirtualGoodsTransactionVO();
                vo.buyItemShortCode = GSBackendKeys.PlayerDetails.COINS;
                vo.buyQuantity = (int)storeItem.currency4Payout;
                vo.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
                vo.consumeQuantity = storeItem.currency3Payout;
                virtualGoodsTransactionResultSignal.AddOnce(OnVirtualGoodTranscationCompleted);
                virtualGoodsTransactionSignal.Dispatch(vo);
            }
            else
            {
                SpotPurchaseMediator.analyticsContext = $"{storeItem.currency4Payout}_coins_pack_state_1";
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SPOT_PURCHASE);
            }

            analyticsService.Event(AnalyticsEventId.drop_off_area_interaction, AnalyticsContext.out_of_coins_pop_up);
        }

        private void OnVirtualGoodTranscationCompleted(BackendResult res)
        {
            if (res == BackendResult.SUCCESS)
            {
                view.audioService.Play(view.audioService.sounds.SFX_REWARD_UNLOCKED);
            }
        }

        private IEnumerator DispatchCloseSignalAsync()
        {
            yield return new WaitForEndOfFrame();
            var vo = new VirtualGoodsTransactionVO();
            vo.buyItemShortCode = GSBackendKeys.PlayerDetails.COINS;
            vo.buyQuantity = 0;
            OnCoinsPurchased(vo);
        }
    }
}
