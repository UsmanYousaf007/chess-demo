/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantGame
{
	public partial class CPUStoreView
	{
		public Signal<StoreItem> buyButtonClickedSignal = new Signal<StoreItem>();
        public Signal closeButtonClickedSignal = new Signal();

        [Header("ConfirmDlg")]
        public GameObject buyDlg;
        public Text titleLabel;
        public Text itemNameLabel;
        public Button yesButton;
		public Button noButton;
        public Text yesButtonLabel;
        public Text noButtonLabel;

		private StoreItem buyStoreItem;

		public void InitBuy()
		{
			yesButton.onClick.AddListener(OnYesButtonClicked);
			noButton.onClick.AddListener(OnNoButtonClicked);

			titleLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUY_THEME_TITLE);
            yesButtonLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_YES_BUTTON);
            noButtonLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_NO_BUTTON);

			itemNameLabel.color = Colors.YELLOW;
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
