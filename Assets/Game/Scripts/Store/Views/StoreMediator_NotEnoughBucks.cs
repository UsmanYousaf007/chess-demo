/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.Chess;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using strange.extensions.signal.impl;

namespace TurboLabz.InstantGame 
{
	public partial class StoreMediator
	{
        [Inject] public ShowStoreTabSignal showStoreTabSignal { get; set; }

        public void OnRegisterNotEnoughBucks()
		{
			view.InitNotEnoughBucks();
			view.yesNotEnoughBucksButtonClickedSignal.AddListener(OnYesNotEnoughBucksButtonClicked);
			view.closeNotEnoughBucksButtonClickedSignal.AddListener(OnCloseNotEnoughBucksButtonClicked);
		}

		public void OnRemoveNotEnoughBucks()
		{
			view.CleanupNotEnoughBucks();
		}

		[ListensTo(typeof(NavigatorShowViewSignal))]
		public void OnShowNotEnoughBucksDlg(NavigatorViewId viewId)
		{
			if (viewId == NavigatorViewId.NOT_ENOUGH_BUCKS_DLG) 
			{
				view.ShowNotEnoughBucks();
			}
		}

		[ListensTo(typeof(NavigatorHideViewSignal))]
		public void OnHideNotEnoughBucksDlg(NavigatorViewId viewId)
		{
			if (viewId == NavigatorViewId.NOT_ENOUGH_BUCKS_DLG)
			{
				view.HideNotEnoughBucks();
			}
		}

		[ListensTo(typeof(UpdateStoreNotEnoughBucksDlgSignal))]
		public void OnUpdateStoreNotEnoughBucksDlg(StoreItem item)
		{
			view.UpdateStoreNotEnoughBucksDlg(item);
		}

		private void OnYesNotEnoughBucksButtonClicked()
		{
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_STORE);
            showStoreTabSignal.Dispatch(StoreView.StoreTabs.COINS);
		}

		private void OnCloseNotEnoughBucksButtonClicked()
		{
			navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_STORE);
		}
	}
}
