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
		[Inject] public LoadCPUGameSignal loadCPUGameSignal { get; set; }

 		public override void OnRegister()
		{
			view.Init();
			view.backButtonClickedSignal.AddListener(OnBackButtonClicked);
			view.skinItemClickedSignal.AddListener(OnSkinItemClicked);

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

		private void OnBackButtonClicked()
		{
			loadCPUGameSignal.Dispatch();
		}

		private void OnSkinItemClicked(StoreItem item)
		{
			purchaseStoreItemSignal.Dispatch(item.key, false);
		}
    }
}
