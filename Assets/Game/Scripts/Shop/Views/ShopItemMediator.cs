using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class ShopItemMediator : Mediator
    {
        //View injection
        [Inject] public ShopItemView view { get; set; }

        public override void OnRegister()
        {
            view.Init();
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnStoreAvailable(isAvailable);
        }
    }
}
