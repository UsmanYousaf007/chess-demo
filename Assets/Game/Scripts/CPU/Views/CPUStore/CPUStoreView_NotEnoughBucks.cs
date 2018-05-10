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

		public Button buyNotEnoughBucksButton;
		public Button closeNotEnoughBucksButton;
		public GameObject notEnoughBucksDlg;
		public GameObject notEnoughBucksGallery;

		public Text notEnoughBucksHeadingLabel;
		public Text notEnoughBucksSubHeadingLabel;
		public Text notEnoughBucksBuyLabel;

		public Signal<StoreItem> buyNotEnoughBucksButtonClickedSignal = new Signal<StoreItem>();
		public Signal closeNotEnoughBucksButtonClickedSignal = new Signal();

		private StoreItem notEnoughBucksStoreItem;
		private BuckPackItemPrefab notEnoughBucksOfferPrefab;

		public void InitNotEnoughBucks()
		{
			buyNotEnoughBucksButton.onClick.AddListener(OnBuyNotEnoughBucksButtonClicked);
			closeNotEnoughBucksButton.onClick.AddListener(OnCloseNotEnoughBucksButtonClicked);

			notEnoughBucksHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_TITLE);
			notEnoughBucksSubHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_SUB_HEADING);
			notEnoughBucksBuyLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_BUY);

			notEnoughBucksOfferPrefab = Instantiate<BuckPackItemPrefab>(buckPackItemPrefab);
			notEnoughBucksOfferPrefab.transform.SetParent(notEnoughBucksGallery.transform, false);
		}

		public void CleanupNotEnoughBucks() 
		{
			buyNotEnoughBucksButton.onClick.RemoveAllListeners();
			closeNotEnoughBucksButton.onClick.RemoveAllListeners();
		}

		public void UpdateStoreNotEnoughBucksDlg(StoreItem item)
		{
			notEnoughBucksStoreItem = item;
			notEnoughBucksOfferPrefab.displayName.text = item.displayName;
			notEnoughBucksOfferPrefab.payout.text = item.currency2Payout.ToString();
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

		void OnBuyNotEnoughBucksButtonClicked()
		{
			buyNotEnoughBucksButtonClickedSignal.Dispatch(notEnoughBucksStoreItem);
		}

		void OnCloseNotEnoughBucksButtonClicked()
		{
			closeNotEnoughBucksButtonClickedSignal.Dispatch();
		}
	}
}
