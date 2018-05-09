﻿/// @license Propriety <http://license.url>
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

		public Signal yesButtonClickedSignal = new Signal();
		public Signal noButtonClickedSignal = new Signal();

		public GameObject uiBlocker;
		public GameObject buyDlg;

		public Text titleLabel;
		public Text yesButtonLabel;
		public Text noButtonLabel;
		public Text itemNameLabel;
		public Text priceLabel;
		public Text forLabel;

		public void InitBuy()
		{
			yesButton.onClick.AddListener(OnYesButtonClicked);
			noButton.onClick.AddListener(OnNoButtonClicked);

			titleLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUY_THEME_TITLE);
			yesButtonLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUY_YES_BUTTON);
			noButtonLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUY_NO_BUTTON);
			forLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUY_FOR);
		}

		public void CleanupBuy() 
		{
			yesButton.onClick.RemoveAllListeners();
			noButton.onClick.RemoveAllListeners();
		}

		public void ShowBuy()
		{
			itemNameLabel.text = activeStoreItem.displayName;
			priceLabel.text = activeStoreItem.currency2Cost.ToString();

			uiBlocker.SetActive(true);
			buyDlg.SetActive(true);
		}

		public void HideBuy()
		{
			uiBlocker.SetActive(false);
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
			yesButtonClickedSignal.Dispatch();
		}

		void OnNoButtonClicked()
		{
			noButtonClickedSignal.Dispatch();
		}
	}
}
