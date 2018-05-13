/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
	public partial class CPUStoreView
	{
		[Header("ConfirmDlg")]

		public Button yesButton;
		public Button noButton;

		public Signal<StoreItem> buyButtonClickedSignal = new Signal<StoreItem>();
		public Signal closeButtonClickedSignal = new Signal();

		public GameObject uiBlocker;
		public GameObject buyDlg;

		public Text titleLabel;
		public Text buyButtonLabel;
		public Text itemNameLabel;
		public Text priceLabel;

		private StoreItem buyStoreItem;

		public void InitBuy()
		{
			yesButton.onClick.AddListener(OnYesButtonClicked);
			noButton.onClick.AddListener(OnNoButtonClicked);

			titleLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUY_THEME_TITLE);
			buyButtonLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUY_BUY_BUTTON);
		}

		public void CleanupBuy() 
		{
			yesButton.onClick.RemoveAllListeners();
			noButton.onClick.RemoveAllListeners();
		}

		public void UpdateStoreBuyDlg(StoreItem item)
		{
			buyStoreItem = item;
			itemNameLabel.text = item.displayName;
			priceLabel.text = item.currency2Cost.ToString();
		}

		public void ShowBuy()
		{
			buyDlg.SetActive(true);
		}

		public void HideBuy()
		{
			buyDlg.SetActive(false);
		}

		public void OnEscapeClicked()
		{
			if (buyDlg.activeSelf)
			{
				HideBuy();
			}
		}

		void OnYesButtonClicked()
		{
			buyButtonClickedSignal.Dispatch(buyStoreItem);
		}

		void OnNoButtonClicked()
		{
			closeButtonClickedSignal.Dispatch();
		}
	}
}
