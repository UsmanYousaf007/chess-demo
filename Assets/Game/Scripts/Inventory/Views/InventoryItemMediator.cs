﻿using strange.extensions.mediation.impl;

namespace TurboLabz.InstantFramework
{
    public class InventoryItemMediator : Mediator
    {
        //View injection
        [Inject] public InventoryItemView view { get; set; }

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

        [ListensTo(typeof(UpdatePlayerInventorySignal))]
        public void OnInventoryUpdated(PlayerInventoryVO inventory)
        {
            view.SetupPriceAndCount();
        }
    }
}
