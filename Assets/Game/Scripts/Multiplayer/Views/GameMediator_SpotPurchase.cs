using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

namespace TurboLabz.Multiplayer
{
    public partial class GameMediator
    {
        [Inject] public LoadSpotPurchaseSignal loadSpotPurchaseSignal { get; set; }
        [Inject] public UpdatePlayerInventorySignal updatePlayerInventorySignal { get; set; }

        public void OnRegisterSpotPurchase()
        {
            view.openSpotPurchaseSignal.AddListener(OnOpenSpotPurchase);
        }

        void OnOpenSpotPurchase(SpotPurchaseView.PowerUpSections activeSection)
        {
            loadSpotPurchaseSignal.Dispatch(activeSection);
        }

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnUpdatePlayerInventory(PlayerInventoryVO vo)
        {
            if (view.IsVisible())
            {
                view.UpdateHintCount(vo.hintCount);
                view.UpdateSafeMoveCount(vo.safeMoveCount, true);
                view.UpdateHindsightCount(vo.hindsightCount);
            }
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

        [ListensTo(typeof(ChessboardBlockerEnableSignal))]
        public void OnChessboardBlockerEnable(bool isEnaable)
        {
            if(isEnaable)
            {
                view.EnableModalBlocker();
            }
            else
            {
                view.DisableModalBlocker();
            }
            
            //view.chessboardBlocker.SetActive(isEnaable);
        }
    }
}