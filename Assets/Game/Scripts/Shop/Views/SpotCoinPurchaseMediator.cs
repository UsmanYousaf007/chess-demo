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
            adView = false;
        }

        [ListensTo(typeof(UpdateSpotCoinsWatchAdDlgSignal))]
        public void OnUpdateAdView(long betValue, StoreItem storeItem, AdPlacements adPlacement)
        {
            this.betValue = betValue;
            this.adPlacement = adPlacement;
            view.UpdateAdDlg(storeItem);
            adView = true;
        }

        [ListensTo(typeof(VirtualGoodBoughtSignal))]
        public void OnCoinsPurchased(string shortCode, int quantity)
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

                if (quantity > 0)
                {
                    var state = adView ? 1 : 2;
                    analyticsService.Event(AnalyticsEventId.coin_popup_purchase, AnalyticsParameter.context, $"{quantity}_coins_pack_state_{state}");
                    analyticsService.ResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.COINS, quantity, "spot_purchase", $"coins_{quantity}_state_{state}");
                }
            }
        }

        private void OnCloseDlgSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnWatchAdSignal()
        {
            showRewardedAdSignal.Dispatch(adPlacement);
        }

        [ListensTo(typeof(RewardedVideoResultSignal))]
        public void OnRewardClaimed(AdsResult result, AdPlacements adPlacement)
        {
            if (view.isActiveAndEnabled && adPlacement == this.adPlacement)
            {
                switch (result)
                {
                    case AdsResult.FINISHED:
                        view.audioService.Play(view.audioService.sounds.SFX_REWARD_UNLOCKED);
                        OnCoinsPurchased(GSBackendKeys.PlayerDetails.COINS, 0);
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
