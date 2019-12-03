using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class ConfirmDlgMediator : Mediator
    {
        // View injection
        [Inject] public ConfirmDlgView view { get; set; }

        //Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.closeSignal.AddListener(OnClose);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CONFIRM_DLG)
            {
                view.Show();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideView(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CONFIRM_DLG)
            {
                view.Hide();
            }
        }

        [ListensTo(typeof(UpdateConfirmDlgSignal))]
        public void OnUpdateDlg(ConfirmDlgVO vo)
        {
            view.UpdateDlg(vo);
        }

        void OnClose()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
        }
    }
}
