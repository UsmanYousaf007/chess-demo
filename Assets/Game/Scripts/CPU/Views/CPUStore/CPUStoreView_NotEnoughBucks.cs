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
		[Header("NotEnoughBucksDlg")]

		public GameObject notEnoughBucksDlg;

		public Button closeNotEnoughBucksButton;
		public Button yesNotEnoughBucksButton;

		public Text notEnoughBucksHeadingLabel;
		public Text notEnoughBucksSubHeadingLabel;
		public Text notEnoughBucksYesButtonLabel;

		public Signal yesNotEnoughBucksButtonClickedSignal = new Signal();
		public Signal closeNotEnoughBucksButtonClickedSignal = new Signal();

		public void InitNotEnoughBucks()
		{
			closeNotEnoughBucksButton.onClick.AddListener(OnCloseNotEnoughBucksButtonClicked);
			yesNotEnoughBucksButton.onClick.AddListener(OnYesNotEnoughBucksButtonClicked);

			notEnoughBucksHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_TITLE);
			notEnoughBucksSubHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_SUB_HEADING);
			notEnoughBucksYesButtonLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_YES_BUTTON);
		}

		public void CleanupNotEnoughBucks() 
		{
			closeNotEnoughBucksButton.onClick.RemoveAllListeners();
		}

		public void UpdateStoreNotEnoughBucksDlg(StoreItem item)
		{
		}

		public void ShowNotEnoughBucks()
		{
			uiBlocker.SetActive(true);
			notEnoughBucksDlg.SetActive(true);
		}

		public void HideNotEnoughBucks()
		{
			uiBlocker.SetActive(false);
			notEnoughBucksDlg.SetActive(false);
		}

		public void OnNotEnoughBucksEscapeClicked()
		{
			if (notEnoughBucksDlg.activeSelf)
			{
				HideNotEnoughBucks();
			}
		}

		void OnYesNotEnoughBucksButtonClicked()
		{
			yesNotEnoughBucksButtonClickedSignal.Dispatch();
		}

		void OnCloseNotEnoughBucksButtonClicked()
		{
			closeNotEnoughBucksButtonClickedSignal.Dispatch();
		}
	}
}
