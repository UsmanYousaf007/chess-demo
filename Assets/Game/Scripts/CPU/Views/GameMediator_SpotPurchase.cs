﻿using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU
{
    public partial class GameMediator
    {
        [Inject] public LoadSpotPurchaseSignal loadSpotPurchaseSignal { get; set; }

        public void OnRegisterSpotPurchase()
        {
            view.openSpotPurchaseSignal.AddListener(OnOpenSpotPurchase);
        }

        void OnOpenSpotPurchase(SpotPurchaseView.PowerUpSections activeSection)
        {
            loadSpotPurchaseSignal.Dispatch(activeSection);
        }

        [ListensTo(typeof(NavigatorShowViewSignal))]
        public void OnShowSpotPurchase(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
            {
                view.chessboardBlocker.SetActive(true);
            }
        }

        [ListensTo(typeof(NavigatorHideViewSignal))]
        public void OhHideSpotPurchase(NavigatorViewId viewId)
        {
            if (viewId == NavigatorViewId.SPOT_PURCHASE_DLG)
            {
                view.chessboardBlocker.SetActive(false);
            }
        }
    }
}
