using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public class SpotPurchaseMediator : Mediator
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public UpdateStoreBuyDlgSignal updateStoreBuyDlgSignal { get; set; }

        // View injection
        [Inject] public SpotPurchaseView view { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

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

        [ListensTo(typeof(UpdateSpotPurchaseSignal))]
        public void OnUpdateStore(StoreVO vo, SpotPurchaseView.PowerUpSections activeSection)
        {
            view.UpdateView(vo, activeSection);
        }

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnPurchaseResult(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.PERMISSION_TO_PURCHASE)
            {
                updateStoreBuyDlgSignal.Dispatch(item);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_BUY_DLG);
            }
            else if (result == PurchaseResult.PURCHASE_SUCCESS )
            {
                OnCloseClicked();
            }
        }

        private void OnCloseClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }

        private void OnStoreItemClicked(StoreItem item)
        {
            analyticsService.TapShopItem(item.displayName);

            // Purchase item after confirmation. No confirmation for remote store items
            purchaseStoreItemSignal.Dispatch(item.key, item.remoteProductId != null);
        }
    }
}
