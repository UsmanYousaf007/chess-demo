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
		[Inject] public LoadStoreSignal loadStoreSignal { get; set; }

		public void OnRegisterBuy()
		{
			view.InitBuy();
			view.closeButtonClickedSignal.AddListener(OnCloseButtonClicked);
			view.buyButtonClickedSignal.AddListener(OnBuyButtonClicked);
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

		private void OnCloseButtonClicked()
		{
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_STORE);
		}

		private void OnBuyButtonClicked(StoreItem item)
		{
            // Make the confirmed purchase
            purchaseResultSignal.AddListener(OnBuyPurchaseResult);
			purchaseStoreItemSignal.Dispatch(item.key, true);
		}

        private void OnBuyPurchaseResult(StoreItem item, PurchaseResult result)
        {
            purchaseResultSignal.RemoveListener(OnBuyPurchaseResult);

            if (result == PurchaseResult.PURCHASE_FAILURE)
            {
                // TODO: Show a message to user in case of purchase failure
            }

            loadStoreSignal.Dispatch();
        }
	}
}
