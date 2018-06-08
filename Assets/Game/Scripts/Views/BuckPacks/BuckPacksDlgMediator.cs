/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantChess 
{
	public class BuckPacksDlgMediator : Mediator
	{
		[Inject] public BuckPacksDlgView view { get; set; }

		[Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
		[Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }

		public override void OnRegister()
		{
			view.InitBuckPacks();
			view.closeBuckPacksButtonClickedSignal.AddListener(OnCloseBuckPacksButtonClicked);
			view.buckPacksClickedSignal.AddListener(OnBuckPackClicked);
		}

		public override void OnRemove()
		{
			view.CleanupBuckPacks();
		}

		[ListensTo(typeof(NavigatorShowViewSignal))]
		public void OnShowBuckPacksDlg(NavigatorViewId viewId)
		{
			if (viewId == NavigatorViewId.BUCK_PACKS_DLG) 
			{
				view.ShowBuckPacks();
			}
		}

		[ListensTo(typeof(NavigatorHideViewSignal))]
		public void OnHideBuckPacksDlg(NavigatorViewId viewId)
		{
			if (viewId == NavigatorViewId.BUCK_PACKS_DLG)
			{
				view.HideBuckPacks();
			}
		}

		[ListensTo(typeof(UpdateStoreBuckPacksDlgSignal))]
		public void OnUpdateStoreBuckPacksDlg(StoreVO vo)
		{
			view.UpdateStoreBuckPacksDlg(vo);
		}

		private void OnBuckPackClicked(StoreItem item)
		{
			purchaseStoreItemSignal.Dispatch(item.key, true);
			navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
		}

		private void OnCloseBuckPacksButtonClicked()
		{
			navigatorEventSignal.Dispatch(NavigatorEvent.ESCAPE);
		}
	}
}
