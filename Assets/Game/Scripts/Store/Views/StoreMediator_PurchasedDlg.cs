/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public partial class StoreMediator
    {
        public void OnRegisterPurchasedDlg()
        {
            view.InitPurchasedDlg();
            view.purchaseDlgOkButtonClickedSignal.AddListener(OnPurchasedDlgOkButtonClicked);
        }

        public void OnRemovePurchasedDlg()
        {
            view.CleanupPurchasedDlg();
            view.purchaseDlgOkButtonClickedSignal.RemoveAllListeners();
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShoPurchasedDlg(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PURCHASED_DLG)
            {
                view.ShowPurchasedDlg();
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OnHidePurchasedDlg(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.PURCHASED_DLG)
            {
                view.HidePurchasedDlg();
            }
        }

        private void OnPurchasedDlgOkButtonClicked()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_STORE);
        }
    }
}
