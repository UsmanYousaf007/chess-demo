using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class ShopMediator : Mediator
    {
        //View Injection
        [Inject] public ShopView view { get; set; }

        //Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

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
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_SUBSCRIPTION_DLG);
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            view.SetSubscriptionOwnedStatus();
        }
    }
}
