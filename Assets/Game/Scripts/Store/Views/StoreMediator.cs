/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
    public partial class StoreMediator : Mediator
    {
        // View injection
		[Inject] public StoreView view { get; set; }

		// Dispatch signals
		[Inject] public LoadLobbySignal loadLobbySignal { get; set; }
		[Inject] public LoadBuckPacksSignal loadBuckPacksSignal { get; set; }
		[Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateStoreBuyDlgSignal updateStoreBuyDlgSignal { get; set; }
        [Inject] public UpdateStoreNotEnoughBucksDlgSignal updateStoreNotEnoughBucksDlgSignal { get; set; }
        [Inject] public SetSkinSignal setSkinSignal { get; set; }
        [Inject] public SavePlayerInventorySignal savePlayerInventorySignal { get; set; }

 		public override void OnRegister()
		{
			view.Init();
			view.skinItemClickedSignal.AddListener(OnSkinItemClicked);

			OnRegisterBuy();
			OnRegisterNotEnoughBucks();
		}

		public override void OnRemove()
		{
			view.skinItemClickedSignal.RemoveAllListeners ();

			OnRemoveBuy();
			OnRemoveNotEnoughBucks();
		}

		[ListensTo(typeof(NavigatorShowViewSignal))]
		public void OnShowView(NavigatorViewId viewId)
		{
			if (viewId == NavigatorViewId.STORE) 
			{
				view.Show();
			}
		}

		[ListensTo(typeof(NavigatorHideViewSignal))]
		public void OnHideView(NavigatorViewId viewId)
		{
			if (viewId == NavigatorViewId.STORE)
			{
                if (view.HasSkinChanged())
                {
                    // Dispatch save skin signal
                    savePlayerInventorySignal.Dispatch();
                }

				view.Hide();
			}
		}

		[ListensTo(typeof(UpdateStoreSignal))]
		public void OnUpdateStore(StoreVO vo)
		{
			view.UpdateView(vo);
		}

		[ListensTo(typeof(GameAppEventSignal))]
		public void OnAppEvent(AppEvent evt)
		{
			if (!view || !view.IsVisible())
			{
				return;
			}

			if (evt == AppEvent.PAUSED || evt == AppEvent.QUIT)
			{
                return;
			}
		}

        [ListensTo(typeof(PurchaseStoreItemResultSignal))]
        public void OnPurchaseResult(StoreItem item, PurchaseResult result)
        {
            if (result == PurchaseResult.ALREADY_OWNED || result == PurchaseResult.PURCHASE_SUCCESS)
            {
                view.currentSkinItemId = item.key;
                view.UpdateItemThumbnail(item.key);

                setSkinSignal.Dispatch(item.key);
            }
            else if (result == PurchaseResult.NOT_ENOUGH_BUCKS)
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_NOT_ENOUGH_DLG);
            }
            else if (result == PurchaseResult.PERMISSION_TO_PURCHASE)
            {
                updateStoreBuyDlgSignal.Dispatch(item);
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_BUY_DLG);
            }
        }

		public void OnAddBucksButtonClicked()
		{
			loadBuckPacksSignal.Dispatch();
		}

		private void OnSkinItemClicked(StoreItem item)
		{
			// Purchase item after confirmation 
			purchaseStoreItemSignal.Dispatch(item.key, false);
		}

        private void OnBackButtonClicked()
        {
            loadLobbySignal.Dispatch();
        }
    }
}
