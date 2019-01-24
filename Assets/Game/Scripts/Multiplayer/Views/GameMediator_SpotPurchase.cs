using TurboLabz.InstantFramework;
using TurboLabz.InstantGame;

namespace TurboLabz.Multiplayer
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
    }
}
