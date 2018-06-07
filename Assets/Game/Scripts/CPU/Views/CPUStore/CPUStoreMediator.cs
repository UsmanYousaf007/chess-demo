/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using strange.extensions.mediation.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public partial class CPUStoreMediator : Mediator
    {
        // View injection
		[Inject] public CPUStoreView view { get; set; }

		// Dispatch signals
		[Inject] public LoadLobbySignal loadLobbySignal { get; set; }
		[Inject] public LoadBuckPacksSignal loadBuckPacksSignal { get; set; }
		[Inject] public ApplySkinSignal applySkinSignal { get; set; }
		[Inject] public UpdateSkinSignal updateSkinSignal { get; set; }
        [Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateStoreBuyDlgSignal updateStoreBuyDlgSignal { get; set; }
        [Inject] public UpdateStoreNotEnoughBucksDlgSignal updateStoreNotEnoughBucksDlgSignal { get; set; }

        // Listen to signals
        [Inject] public PurchaseStoreItemResultSignal purchaseResultSignal { get; set; }

 		public override void OnRegister()
		{
			view.Init();
			view.backButtonClickedSignal.AddListener(OnBackButtonClicked);
			view.skinItemClickedSignal.AddListener(OnSkinItemClicked);
			view.addBucksButtonClickedSignal.AddListener(OnAddBucksButtonClicked);

			OnRegisterBuy();
			OnRegisterNotEnoughBucks();
		}

		public override void OnRemove()
		{
			view.backButtonClickedSignal.RemoveAllListeners();
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
				view.Hide();
			}
		}

		[ListensTo(typeof(UpdateStoreSignal))]
		public void OnUpdateStore(CPUStoreVO vo)
		{
			view.UpdateView(vo);
		}

		[ListensTo(typeof(UpdatePlayerBucksDisplaySignal))]
		public void OnUpdatePlayerBucksDisplay(long playerBucks)
		{
			view.UpdatePlayerBucks(playerBucks);
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

		private void OnBackButtonClicked()
		{
			loadLobbySignal.Dispatch();
		}

		public void OnAddBucksButtonClicked()
		{
			loadBuckPacksSignal.Dispatch();
		}

		private void OnSkinItemClicked(StoreItem item)
		{
			// Purchase item after confirmation 
            purchaseResultSignal.AddListener(OnPurchaseResult);
			purchaseStoreItemSignal.Dispatch(item.key, false);
		}

        private void OnPurchaseResult(StoreItem item, PurchaseResult result)
        {
            purchaseResultSignal.RemoveListener(OnPurchaseResult);

            if (result == PurchaseResult.ALREADY_OWNED)
            {
                applySkinSignal.Dispatch(item.key);
                updateSkinSignal.Dispatch();
            }
            else if (result == PurchaseResult.NOT_ENOUGH_BUCKS)
            {
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_NOT_ENOUGH_DLG);
            }
            else if (result == PurchaseResult.PERMISSION_TO_PURCHASE)
            {
                updateStoreBuyDlgSignal.Dispatch(item);
                navigatorEventSignal.Dispatch (NavigatorEvent.SHOW_BUY_DLG);
            }
        }
    }
}
