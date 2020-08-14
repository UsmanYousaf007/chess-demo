using strange.extensions.mediation.impl;
using UnityEngine;

namespace TurboLabz.InstantFramework
{
    public class ShopItemMediator : Mediator
    {
        //View injection
        [Inject] public ShopItemView view { get; set; }

        //Dispatch Signals
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }

        public override void OnRegister()
        {
            view.Init();
            view.buyButtonSignal.AddListener(OnPurchaseSignal);
        }

        [ListensTo(typeof(StoreAvailableSignal))]
        public void OnStoreAvailable(bool isAvailable)
        {
            view.OnStoreAvailable(isAvailable);
        }

        private void OnPurchaseSignal(string shortCode)
        {
            purchaseStoreItemSignal.Dispatch(shortCode, true);
        }

        [ListensTo(typeof(UpdatePurchasedStoreItemSignal))]
        public void OnSubscriptionPurchased(StoreItem item)
        {
            view.SetOwnedStatus();

            if (view.checkOwned && view.shortCode.Equals(item.key))
            {
                iTween.PunchScale(view.owned.gameObject, iTween.Hash("amount", new Vector3(0.3f, 0.3f, 0f), "time", 3f));
                view.audioService.Play(view.audioService.sounds.SFX_REWARD_UNLOCKED);
            }
        }
    }
}
