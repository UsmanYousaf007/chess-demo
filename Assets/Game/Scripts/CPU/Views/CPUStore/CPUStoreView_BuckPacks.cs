/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantChess
{
	public partial class CPUStoreView
	{
		[Header("BuckPacksDlg")]

		public Button closeBuckPacksButton;
		public GameObject buckPacksDlg;
		public GameObject buckPacksGallery;
		public BuckPackItemPrefab buckPackItemPrefab;

		public Text buckPacksHeadingLabel;
		public Text buckPacksSubHeadingLabel;

		public Signal<StoreItem> buyBuckPacksButtonClickedSignal = new Signal<StoreItem>();
		public Signal closeBuckPacksButtonClickedSignal = new Signal();

		StoreItem buckPacksStoreItem;
		List<BuckPackItemPrefab> buckPackPrefabs = null;

		public void InitBuckPacks()
		{
			closeBuckPacksButton.onClick.AddListener(OnCloseBuckPacksButtonClicked);

			buckPacksHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_TITLE);
			buckPacksSubHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_NOT_ENOUGH_BUCKS_SUB_HEADING);
		}

		public void CleanupBuckPacks() 
		{
			closeBuckPacksButton.onClick.RemoveAllListeners();
		}

		public void UpdateStoreBuckPacksDlg(CPUStoreVO vo)
		{
			if (buckPackPrefabs == null) 
			{
				CreateBuckPackPrefabs(vo);
				PopulateBuckPacks(vo);
			}
		}

		public void ShowBuckPacks()
		{
			uiBlocker.SetActive(true);
			buckPacksDlg.SetActive(true);
		}

		public void HideBuckPacks()
		{
			uiBlocker.SetActive(false);
			buckPacksDlg.SetActive(false);
		}

		public void OnBuckPacksEscapeClicked()
		{
			if (buckPacksDlg.activeSelf)
			{
				HideBuckPacks();
			}
		}

		void OnBuckPackItemClicked(StoreItem item)
		{
			LogUtil.Log ("OnBuckPackItemClicked " + item.displayName, "cyan");
			buyBuckPacksButtonClickedSignal.Dispatch(item);
		}

		void OnCloseBuckPacksButtonClicked()
		{
			closeBuckPacksButtonClickedSignal.Dispatch();
		}

		private void CreateBuckPackPrefabs(CPUStoreVO vo)
		{
			IStoreSettingsModel storeSettingsModel = vo.storeSettingsModel;
			BuckPackItemPrefab prefab = Instantiate<BuckPackItemPrefab>(buckPackItemPrefab);

			buckPackPrefabs = new List<BuckPackItemPrefab>();

			List<StoreItem> list = storeSettingsModel.lists["BuckPack"];
			foreach (StoreItem item in list) 
			{
				BuckPackItemPrefab buckPackThumbnail = Object.Instantiate(prefab);
				buckPackThumbnail.transform.SetParent(buckPacksGallery.transform, false);
				buckPackPrefabs.Add(buckPackThumbnail);
			}

			Destroy(prefab);
		}

		private void PopulateBuckPacks(CPUStoreVO vo)
		{
			IStoreSettingsModel storeSettingsModel = vo.storeSettingsModel;

			List<StoreItem> list = storeSettingsModel.lists["BuckPack"];
			for (int i = 0; i < list.Count; i++) 
			{
				BuckPackItemPrefab thumbnail = buckPackPrefabs[i];
				StoreItem item = list[i];

				thumbnail.displayName.text = item.displayName;
				thumbnail.payout.text = item.currency2Payout.ToString();
				thumbnail.button.onClick.AddListener(() => OnBuckPackItemClicked(item));
			}
		}
	}
}
