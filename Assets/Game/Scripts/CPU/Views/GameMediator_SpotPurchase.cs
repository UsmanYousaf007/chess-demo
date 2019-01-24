using TurboLabz.InstantFramework;
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

        void OnOpenSpotPurchase()
        {
            loadSpotPurchaseSignal.Dispatch();
        }
    }
}
