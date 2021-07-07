using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class ShopMediator : Mediator
    {
        //View Injection
        [Inject] public ShopView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IPromotionsService promotionsService { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateShopBundlePurchasedViewSignal updateShopBundlePurchasedViewSignal { get; set; }
        [Inject] public UpdatePurchaseSuccessDlgSignal updatePurchaseSuccessDlgSignal { get; set; }
        [Inject] public ShopVistedSignal shopVistedSignal { get; set; }

        //Models
        [Inject] public IStoreSettingsModel storeSettingsModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.subscriptionButtonClickedSignal.AddListener(OnSubscriptionButtonClickedSignal);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOP)
            {
                shopVistedSignal.Dispatch();
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.shop);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOP)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnStoreAvailable(isAvailable);
        }

        private void OnSubscriptionButtonClickedSignal()
        {
            promotionsService.LoadSubscriptionPromotion();
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            view.SetSubscriptionOwnedStatus();
            //view.SetBundle();

            if (view.isActiveAndEnabled && item.kind.Equals(GSBackendKeys.ShopItem.SPECIALPACK_SHOP_TAG))
            {
                //navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SHOP_BUNDLE_PURCHASED);
                //updateShopBundlePurchasedViewSignal.Dispatch(item);

                //analytics
                var context = item.displayName.Replace(' ', '_').ToLower();
                analyticsService.Event(AnalyticsEventId.shop_purchase, AnalyticsParameter.context, context);
                analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.GEMS, item.currency3Cost, "shop", $"{context}_gems");
                analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.COINS, (int)item.currency4Cost, "shop", $"{context}_coins");
            }
            else if (view.isActiveAndEnabled && item.kind.Equals(GSBackendKeys.ShopItem.GEMPACK_SHOP_TAG))
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PURCHASE_SUCCESS_DLG);
                updatePurchaseSuccessDlgSignal.Dispatch(item);
            }
        }

        [ListensTo(typeof(ResetSubscirptionStatusSignal))]
        public void OnResetSubcriptionStatus()
        {
            view.SetSubscriptionOwnedStatus();
        }

        [ListensTo(typeof(ActivePromotionSaleSingal))]
        public void OnShowSale(string key)
        {
            view.SetupSale(key);
        }

        [ListensTo(typeof(VirtualGoodBoughtSignal))]
        public void OnCoinsPurchased(VirtualGoodsTransactionVO transactionVO)
        {
            if (view.isActiveAndEnabled && transactionVO.buyItemShortCode.Equals(GSBackendKeys.PlayerDetails.COINS))
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_PURCHASE_SUCCESS_DLG);
                updatePurchaseSuccessDlgSignal.Dispatch(storeSettingsModel.GetItemByCoinsValue(transactionVO.buyQuantity));
                analyticsService.Event(AnalyticsEventId.shop_purchase, AnalyticsParameter.context, $"{transactionVO.buyQuantity}_coins_pack");
                analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.COINS, transactionVO.buyQuantity, "shop", $"coins_{transactionVO.buyQuantity}");
                analyticsService.ResourceEvent(GAResourceFlowType.Sink, GSBackendKeys.PlayerDetails.GEMS, transactionVO.consumeQuantity, "shop", $"coins_{transactionVO.buyQuantity}");
            }
        }

        [ListensTo(typeof(OutOfGemsSignal))]
        public void OnOutOfGems()
        {
            if (view.isActiveAndEnabled)
            {
                view.ShowGems();
            }
        }
    }
}
