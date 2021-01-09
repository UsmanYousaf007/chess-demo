using System.Collections.Generic;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class SpotCoinPurchaseMediator : Mediator
    {
        //View Injection
        [Inject] public SpotCoinPurchaseView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateTimeSelectDlgSignal updateTimeSelectDlgSignal { get; set; }
        [Inject] public VirtualGoodsTransactionSignal virtualGoodsTransactionSignal { get; set; }
        [Inject] public VirtualGoodsTransactionResultSignal virtualGoodsTransactionResultSignal { get; set; }
        [Inject] public ShowRewardedAdSignal showRewardedAdSignal { get; set; }
                [Inject] public LoadCareerCardSignal loadCareerCardSignal { get; set; }

        private long betValue;

        public override void OnRegister()
        {
            view.Init();
            view.closeDlgSignal.AddListener(OnCloseDlgSignal);
            view.buyButtonClickedSignal.AddListener(OnBuySignal);
            view.watchAdButtonClickedSignal.AddListener(OnWatchAdSignal);
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
            }
        }
        
        [ListensTo(typeof(UpdateSpotCoinsPurchaseDlgSignal))]
        public void OnUpdateView(long betValue, List<string> packs)
        {
            this.betValue = betValue;
            view.UpdateView(packs);
        }

        [ListensTo(typeof(UpdateSpotCoinsWatchAdDlgSignal))]
        public void OnUpdateAdView(long betValue, StoreItem storeItem)
        {
            this.betValue = betValue;
            view.UpdateAdDlg(storeItem);
        }

        [ListensTo(typeof(VirtualGoodBoughtSignal))]
        public void OnCoinsPurchased(string shortCode)
        {
            if (view.isActiveAndEnabled && shortCode.Equals(GSBackendKeys.PlayerDetails.COINS))
            {
                OnCloseDlgSignal();

                if (betValue > 0)
                {
                    navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SELECT_TIME_MODE);
                    updateTimeSelectDlgSignal.Dispatch(betValue);
                }
                else
                {
                    loadCareerCardSignal.Dispatch();
                }
            }
        }

        private void OnCloseDlgSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnWatchAdSignal()
        {
            showRewardedAdSignal.Dispatch(AdPlacements.Rewarded_coins_purchase);
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardClaimed(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == AdPlacements.Rewarded_coins_purchase)
            {
                switch (result)
                {
                    case AdsResult.FINISHED:
                        view.audioService.Play(view.audioService.sounds.SFX_REWARD_UNLOCKED);
                        OnCoinsPurchased(GSBackendKeys.PlayerDetails.COINS);
                        break;

                    case AdsResult.NOT_AVAILABLE:
                        view.toolTip.SetActive(true);
                        break;
                }
            }
        }

        private void OnBuySignal(StoreItem storeItem)
        {
            var vo = new VirtualGoodsTransactionVO();
            vo.buyItemShortCode = GSBackendKeys.PlayerDetails.COINS;
            vo.buyQuantity = (int)storeItem.currency4Payout;
            vo.consumeItemShortCode = GSBackendKeys.PlayerDetails.GEMS;
            vo.consumeQuantity = storeItem.currency3Payout;
            virtualGoodsTransactionResultSignal.AddOnce(OnVirtualGoodTranscationCompleted);
            virtualGoodsTransactionSignal.Dispatch(vo);
        }

        private void OnVirtualGoodTranscationCompleted(BackendResult res)
        {
            if (res == BackendResult.SUCCESS)
            {
                view.audioService.Play(view.audioService.sounds.SFX_REWARD_UNLOCKED);
            }
        }
    }
}
