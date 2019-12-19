using HUF.Analytics.API;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;
using IAnalyticsService = TurboLabz.InstantFramework.IAnalyticsService;

namespace TurboLabz.InstantGame
{
    public class SpotPurchaseMediator : Mediator
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public UpdateStoreBuyDlgSignal updateStoreBuyDlgSignal { get; set; }
        [Inject] public ReportLobbyPromotionAnalyticSingal reportLobbyPromotionAnalyticSingal { get; set; }
        [Inject] public ToggleBannerSignal toggleBannerSignal { get; set; }

        // View injection
        [Inject] public SpotPurchaseView view { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        // Models
        [Inject] public IMetaDataModel metaDataModel { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.closeClickedSignal.AddListener(OnCloseClicked);
            view.storeItemClickedSignal.AddListener(OnStoreItemClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
            {
                view.Show();
                toggleBannerSignal.Dispatch(false);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
            {
                view.Hide();
                toggleBannerSignal.Dispatch(true);
            }
        }

        [ListensTo(typeof(UpdateSpotPurchaseSignal))]
        public void OnUpdateStore(StoreVO vo, SpotPurchaseView.PowerUpSections activeSection)
        {
            view.UpdateView(vo, activeSection);
        }

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnPurchaseResult(StoreItem item, PurchaseResult result)
        {
            if (!view.IsVisible())
            {
                return;
            }

            if (result == PurchaseResult.PERMISSION_TO_PURCHASE)
            {
                updateStoreBuyDlgSignal.Dispatch(item);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_BUY_DLG);
            }
            else if (result == PurchaseResult.PURCHASE_SUCCESS )
            {
                OnCloseClicked();
            }

            // Analytics
            if (result == PurchaseResult.PURCHASE_SUCCESS)
            {
                analyticsService.Event(AnalyticsEventId.v1_spot_purchase_complete, AnalyticsParameter.item_id, item.key);

                if (item.key.Contains(GSBackendKeys.ShopItem.HINDSIGHT_SHOP_TAG))
                {
                    reportLobbyPromotionAnalyticSingal.Dispatch(LobbyPromotionKeys.COACH_PURCHASE, AnalyticsEventId.banner_coach_purchase_success);
                }
                else if (item.key.Contains(GSBackendKeys.ShopItem.HINT_SHOP_TAG))
                {
                    reportLobbyPromotionAnalyticSingal.Dispatch(LobbyPromotionKeys.STRENGTH_PURCHASE, AnalyticsEventId.banner_move_meter_purchase_success);
                }
            }
        }

        private void OnCloseClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnStoreItemClicked(StoreItem item)
        {
            var analyticsEvent = AnalyticsMonetizationEvent.Create("attempt", (int)(item.productPrice * 100))
                .ST1("iap_purchase")
                .ST2(item.displayName.Replace(" ", "_").ToLower())
                .ST3("on_spot")
                .Value((int)(item.productPrice * 100));
            HAnalytics.LogMonetizationEvent((AnalyticsMonetizationEvent)analyticsEvent);

            // Purchase item after confirmation. No confirmation for remote store items
            purchaseStoreItemSignal.Dispatch(item.key, true);
        }

        [ListensTo(typeof(ShowIAPProcessingSignal))]
        public void OnShowProcessingUI(bool show)
        {
            view.ShowProcessing(show);
        }

        [ListensTo(typeof(ReportHAnalyticsForPurchaseResult))]
        public void OnReportHAnalytics(string key, string result)
        {
            if (!view.IsVisible())
            {
                return;
            }

            var item = metaDataModel.store.items[key];

            if (item != null)
            {
                var analyticsEvent = AnalyticsMonetizationEvent.Create(result, (int)(item.productPrice * 100))
                .ST1("iap_purchase")
                .ST2(item.displayName.Replace(" ", "_").ToLower())
                .ST3("on_spot")
                .Value((int)(item.productPrice * 100));
                HAnalytics.LogMonetizationEvent((AnalyticsMonetizationEvent)analyticsEvent);
            }
        }
    }
}
