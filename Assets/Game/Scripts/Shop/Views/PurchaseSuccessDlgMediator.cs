using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class PurchaseSuccessDlgMediator : Mediator
    {
        //View Injection
        [Inject] public PurchaseSuccessDlgView view { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.okPressedSignal.AddListener(OnOkPressedSignal);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PURCHASE_SUCCESS_DLG)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PURCHASE_SUCCESS_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdatePurchaseSuccessDlgSignal))]
        public void OnUpdateView(StoreItem item)
        {
            view.UpdateView(item);
        }

        private void OnOkPressedSignal()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
