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

        private string cameFromScreen;
        public static string customContext = string.Empty;

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
                view.Show();
                cameFromScreen = navigatorModel.previousState.ToString();
                cameFromScreen = !string.IsNullOrEmpty(customContext) ? customContext :
                    CollectionsUtil.GetContextFromState(cameFromScreen.Remove(0, cameFromScreen.IndexOf("NS") + 2));
                analyticsService.ScreenVisit(AnalyticsScreen.spot_purchase_dlg);
                analyticsService.Event(AnalyticsEventId.shop_popup_view, AnalyticsParameter.context, $"{cameFromScreen}_gems");
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
            {
                view.Hide();
                customContext = string.Empty;
            }
        }

        private void OnCloseDlgSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnPurchaseSuccess(StoreItem item)
        {
            if (view.isActiveAndEnabled && item.kind.Equals(GSBackendKeys.ShopItem.GEMPACK_SHOP_TAG))
            {
                OnCloseDlgSignal();
                var context = $"{cameFromScreen}_{item.displayName.Replace(' ', '_').ToLower()}";
                analyticsService.Event(AnalyticsEventId.shop_popup_purchase, AnalyticsParameter.context, context);
                analyticsService.ResourceEvent(GAResourceFlowType.Source, "gems", item.currency3Payout, "spot_purchase", context);
            }
        }

        [ListensTo(typeof(ShowProcessingSignal))]
        public void OnShowProcessing(bool blocker, bool processing)
        {
            view.ShowProcessing(blocker, processing);
        }
    }
}
