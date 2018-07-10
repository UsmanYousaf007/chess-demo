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
        [Header("Not Enough Bucks")]

        public GameObject notEnoughBucksDlg;

        public Button notEnoughBucksNoButton;
        public Button notEnoughBucksYesButton;

		public Text notEnoughBucksHeadingLabel;
		public Text notEnoughBucksSubHeadingLabel;
		public Text notEnoughBucksYesButtonLabel;
        public Text notEnoughBucksNoButtonLabel;

		public Signal yesNotEnoughBucksButtonClickedSignal = new Signal();
		public Signal closeNotEnoughBucksButtonClickedSignal = new Signal();

		public void InitNotEnoughBucks()
		{
            notEnoughBucksNoButton.onClick.AddListener(OnCloseNotEnoughBucksButtonClicked);
            notEnoughBucksYesButton.onClick.AddListener(OnYesNotEnoughBucksButtonClicked);

			notEnoughBucksHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_TITLE);
			notEnoughBucksSubHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_SUB_HEADING);
			notEnoughBucksYesButtonLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_YES_BUTTON);
            notEnoughBucksNoButtonLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_NO_BUTTON);

            notEnoughBucksHeadingLabel.color = Colors.RED;
		}

		public void CleanupNotEnoughBucks() 
		{
            notEnoughBucksNoButton.onClick.RemoveAllListeners();
            notEnoughBucksYesButton.onClick.RemoveAllListeners();
		}

		public void UpdateStoreNotEnoughBucksDlg(StoreItem item)
		{
		}

		public void ShowNotEnoughBucks()
		{
			notEnoughBucksDlg.SetActive(true);
		}

		public void HideNotEnoughBucks()
		{
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
