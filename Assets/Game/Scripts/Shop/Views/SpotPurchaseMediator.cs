using GameAnalyticsSDK;
using strange.extensions.mediation.impl;
using TurboLabz.InstantGame;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class SpotPurchaseMediator : Mediator
    {
        //View Injection
        [Inject] public SpotPurchaseView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public ToggleBannerSignal toggleBannerSignal { get; set; }

        //Models
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public static string analyticsContext = string.Empty;

        private long timeAtDlgShown;

        public override void OnRegister()
        {
            view.Init();
            view.closeDlgSignal.AddListener(OnCloseDlgSignal);
            view.showMoreSignal.AddListener(OnShowMoreSignal);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
            {
                view.SetupDynamicContent(playerModel.dynamicGemSpotBundle);
                view.Show();
                analyticsService.ScreenVisit(AnalyticsScreen.spot_purchase_dlg);
                analyticsService.Event(AnalyticsEventId.shop_popup_view, AnalyticsParameter.context, analyticsContext);
                analyticsService.Event("ux_saleonspotgem_shown", CollectionsUtil.GetContextFromString(playerModel.dynamicBundleToDisplay));
                timeAtDlgShown = backendService.serverClock.currentTimestamp;
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
            {
                view.Hide();
                analyticsContext = string.Empty;
            }
        }

        private void OnCloseDlgSignal()
        {
            OnShowMoreSignal(false);
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnShowMoreSignal(bool showMore)
        {
            if (analyticsContext.Equals("hint") ||
                analyticsContext.Equals("cpu_in_game_power_mode"))
            {
                toggleBannerSignal.Dispatch(!showMore);
            }
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnPurchaseSuccess(StoreItem item)
        {
            if (view.isActiveAndEnabled)
            {
                view.SetupDynamicContent(playerModel.dynamicGemSpotBundle);

                //analytics
                var context = item.displayName.Replace(' ', '_').ToLower();
                var gemsPayout = item.kind.Equals(GSBackendKeys.ShopItem.SPECIALPACK_SHOP_TAG) ? item.currency3Cost : item.currency3Payout;
                analyticsService.DesignEvent(AnalyticsEventId.shop_popup_purchase, "context", analyticsContext, context);
                analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.GEMS, gemsPayout, "spot_purchase", $"{analyticsContext}_{context}");

                if (item.currency4Cost > 0)
                {
                    analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.COINS, (int)item.currency4Cost, "spot_purchase", $"{analyticsContext}_{context}");
                }
                //end analytics

                OnCloseDlgSignal();
            }
        }

        [ListensTo(typeof(BuyDynamicBundleClickedSignal))]
        public void OnBuyDynamicBundleClicked()
        {
            if (view.isActiveAndEnabled)
            {
                var context = CollectionsUtil.GetContextFromString(playerModel.dynamicBundleToDisplay);
                var timePreBuyNow = (backendService.serverClock.currentTimestamp - timeAtDlgShown) / 1000.0f;
                analyticsService.Event("ux_saleonspotgem_tapbuynow", context);
                analyticsService.ValueEvent("ux_saleonspotgem_timeprebuynow", context, timePreBuyNow);
            }
        }
    }
}
