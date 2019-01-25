using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

namespace TurboLabz.CPU
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
            view.UpdateHintCount(vo.hintCount);
            view.UpdateSafeMoveCount(vo.safeMoveCount);
            view.UpdateHindsightCount(vo.hindsightCount);
        }
    }
}
