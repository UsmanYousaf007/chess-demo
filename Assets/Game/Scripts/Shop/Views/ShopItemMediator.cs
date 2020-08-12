using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class ShopItemMediator : Mediator
    {
        //View injection
        [Inject] public ShopItemView view { get; set; }
        [Inject] public ShopView shopView { get; set; }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnStoreAvailable(shopView.iconsContainer, shopView.thumbsContainer, isAvailable);
        }
    }
}
