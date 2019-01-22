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
	public partial class StoreView
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

		private StoreItem buyStoreItem;

		public void InitBuy()
		{
			yesButton.onClick.AddListener(OnYesButtonClicked);
			noButton.onClick.AddListener(OnNoButtonClicked);

            yesButtonLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUY_BUY_BUTTON);
		}

		public void CleanupBuy() 
		{
			yesButton.onClick.RemoveAllListeners();
			noButton.onClick.RemoveAllListeners();
		}

		public void UpdateStoreBuyDlg(StoreItem item)
		{
            titleLabel.text = localizationService.Get(LocalizationKey.STORE_CONFIRM_DLG_TITLE_BUY) + " " + item.displayName;
            itemNameLabel.text = item.description;

            buyStoreItem = item;
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
