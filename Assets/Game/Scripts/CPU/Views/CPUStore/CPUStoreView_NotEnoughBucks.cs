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

		public Button closeNotEnoughBucksButton;
		public GameObject notEnoughBucksDlg;
		public GameObject notEnoughBucksGallery;

		public Text notEnoughBucksHeadingLabel;
		public Text notEnoughBucksSubHeadingLabel;

		public Signal<StoreItem> buyNotEnoughBucksButtonClickedSignal = new Signal<StoreItem>();
		public Signal closeNotEnoughBucksButtonClickedSignal = new Signal();

		private StoreItem notEnoughBucksStoreItem;
		private BuckPackItemPrefab notEnoughBucksOfferPrefab;

		public void InitNotEnoughBucks()
		{
			closeNotEnoughBucksButton.onClick.AddListener(OnCloseNotEnoughBucksButtonClicked);

			notEnoughBucksHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_TITLE);
			notEnoughBucksSubHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_SUB_HEADING);

			notEnoughBucksOfferPrefab = Instantiate<BuckPackItemPrefab>(buckPackItemPrefab);
			notEnoughBucksOfferPrefab.transform.SetParent(notEnoughBucksGallery.transform, false);
			notEnoughBucksOfferPrefab.bucksLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUCKS);

			notEnoughBucksOfferPrefab.button.onClick.AddListener(() => OnBuyNotEnoughBucksButtonClicked());
		}

		public void CleanupNotEnoughBucks() 
		{
			closeNotEnoughBucksButton.onClick.RemoveAllListeners();
		}

		public void UpdateStoreNotEnoughBucksDlg(StoreItem item)
		{
			notEnoughBucksStoreItem = item;
			notEnoughBucksOfferPrefab.payout.text = item.currency2Payout.ToString();
			notEnoughBucksOfferPrefab.price.text = item.remoteProductPrice;
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
