/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.CPU
{
    public partial class GameMediator
    {
        [Inject] public ToggleSafeModeSignal toggleSafeModelSignal { get; set; }
        [Inject] public SafeMoveSignal safeMoveSignal { get; set; }

        public void OnRegisterSafeMove()
        {
            view.InitSafeMove();
            view.safeMoveBtnClickedSignal.AddListener(OnSafeMoveBtnClicked);
            view.safeMoveConfirmClickedSignal.AddListener(OnSafeMoveConfirmedClicked);
            view.safeMoveUndoClickedSignal.AddListener(OnSafeMoveUndoClicked);
        }

        void OnSafeMoveBtnClicked()
        {
            toggleSafeModelSignal.Dispatch();
        }

        void OnSafeMoveConfirmedClicked()
        {
            safeMoveSignal.Dispatch(true);
        }

        void OnSafeMoveUndoClicked()
        {
            safeMoveSignal.Dispatch(false);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowSafeMoveDlg(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_SAFE_MOVE_DLG)
            {
                view.ShowSafeMoveDlg();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHideSafeMoveDlg(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.CPU_SAFE_MOVE_DLG)
            {
                view.HideSafeMoveDlg();
            }
        }

        [ListensTo(typeof(UpdateSafeMoveCountSignal))]
        public void OnUpdateSafeMoveCount(int count)
        {
            if (view.IsVisible())
            {
                view.UpdateSafeMoveCount(count, false);
            }
        }

        [ListensTo(typeof(UpdateSafeMoveStateSignal))]
        public void OnUpdateSafeMoveStateSignal(bool on)
        {
            view.ToggleSafeMove(on);
        }
    }
}
