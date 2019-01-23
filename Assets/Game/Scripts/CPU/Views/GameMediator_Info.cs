using TurboLabz.InstantFramework;

namespace TurboLabz.CPU
{
    public partial class GameMediator
    {
        public void OnRegisterInfo()
        {
            view.InitInfo();
            view.closeButtonClickedSignal.AddListener(CloseButtonClicked);
            view.infoButtonClickedSignal.AddListener(InfoButtonClicked);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowInfo(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_INFO_DLG)
            {
                view.ShowInfo();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideInfo(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_INFO_DLG)
            {
                view.HideInfo();
            }
        }

        void InfoButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_INFO_DLG);
        }

        void CloseButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU);
        }
    }
}
