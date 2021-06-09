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

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        //Models
        [Inject] public INavigatorModel navigatorModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public static string analyticsContext = string.Empty;

        public override void OnRegister()
        {
            view.Init();
            view.closeDlgSignal.AddListener(OnCloseDlgSignal);
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
                //view.finePrint.enabled = cameFromScreen.Contains("tournament");
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
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnPurchaseSuccess(StoreItem item)
        {
            if (view.isActiveAndEnabled)
            {
                view.SetupDynamicContent(playerModel.dynamicGemSpotBundle);

                //analytics
                var context = $"{item.displayName.Replace(' ', '_').ToLower()}";
                analyticsService.DesignEvent(AnalyticsEventId.shop_popup_purchase, "context", analyticsContext, context);
                analyticsService.ResourceEvent(GAResourceFlowType.Source, GSBackendKeys.PlayerDetails.GEMS, item.currency3Payout, "spot_purchase", $"{analyticsContext}_{context}");
                OnCloseDlgSignal();
            }
        }
    }
}
