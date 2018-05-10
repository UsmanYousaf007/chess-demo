/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantChess 
{
	public partial class CPUStoreMediator
	{
		public void OnRegisterBuckPacks()
		{
			view.InitBuckPacks();
			view.closeBuckPacksButtonClickedSignal.AddListener(OnCloseBuckPacksButtonClicked);
			view.buckPacksClickedSignal.AddListener(OnBuckPackClicked);
		}

		public void OnRemoveBuckPacks()
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
		public void OnUpdateStoreBuckPacksDlg(CPUStoreVO vo)
		{
			view.UpdateStoreBuckPacksDlg(vo);
		}

		private void OnBuckPackClicked(StoreItem item)
		{
			purchaseStoreItemSignal.Dispatch(item.key, true);
			loadStoreSignal.Dispatch();
		}

		private void OnCloseBuckPacksButtonClicked()
		{
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_STORE);
		}
	}
}
