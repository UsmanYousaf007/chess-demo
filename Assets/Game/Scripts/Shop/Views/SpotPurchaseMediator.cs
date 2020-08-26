using strange.extensions.mediation.impl;

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
                analyticsService.ScreenVisit(AnalyticsScreen.spot_purchase_dlg);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
            {
                view.Hide();
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
            }
        }
    }
}
