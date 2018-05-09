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
		// Dispatch Signals
		[Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
		[Inject] public PurchaseStoreItemSignal purchaseStoreItemSignal { get; set; }
		[Inject] public LoadStoreSignal loadStoreSignal { get; set; }

		public void OnRegisterBuy()
		{
			view.InitBuy();
			view.noButtonClickedSignal.AddListener(OnNoButtonClicked);
			view.yesButtonClickedSignal.AddListener(OnYesButtonClicked);
		}

		public void OnRemoveBuy()
		{
			view.CleanupBuy();
		}

		[ListensTo(typeof(NavigatorShowViewSignal))]
		public void OnShowBuyView(NavigatorViewId viewId)
		{
			if (viewId == NavigatorViewId.BUY_DLG) 
			{
				view.ShowBuy();
			}
		}

		[ListensTo(typeof(NavigatorHideViewSignal))]
		public void OnHideBuyView(NavigatorViewId viewId)
		{
			if (viewId == NavigatorViewId.BUY_DLG)
			{
				view.HideBuy();
			}
		}

		[ListensTo(typeof(UpdateStoreBuyDlgSignal))]
		public void OnUpdateStoreBuyDlg(StoreItem item)
		{
			view.UpdateStoreBuyDlg(item);
		}

		private void OnNoButtonClicked()
		{
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_STORE);
		}

		private void OnYesButtonClicked(StoreItem item)
		{
			purchaseStoreItemSignal.Dispatch(item.key, true);
			loadStoreSignal.Dispatch();
		}
	}
}
