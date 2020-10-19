using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class ShopBundlePurchasedMediator : Mediator
    {
        //View Injection
        [Inject] public ShopBundlePurchasedView view { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.closeDlgSignal.AddListener(OnCloseDailogue);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOP_BUNDLE_PURCHASED)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SHOP_BUNDLE_PURCHASED)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateShopBundlePurchasedViewSignal))]
        public void OnUpdateView(StoreItem storeItem)
        {
            view.UpdateView(storeItem);
        }

        private void OnCloseDailogue()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
