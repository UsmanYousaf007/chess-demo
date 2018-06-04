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
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantChess
{
	public class BuckPacksDlgView : View
	{
		[Inject] public ILocalizationService localizationService { get; set; }

		[Header("BuckPacksDlg")]
		public Button closeBuckPacksButton;
		public GameObject buckPacksDlg;
		public GameObject buckPacksGallery;
		public BuckPackItemPrefab buckPackItemPrefab;

		public Text buckPacksHeadingLabel;

		public Signal<StoreItem> buckPacksClickedSignal = new Signal<StoreItem>();
		public Signal closeBuckPacksButtonClickedSignal = new Signal();

		StoreItem buckPacksStoreItem;
		Dictionary<string, BuckPackItemPrefab> buckPackPrefabs = null;

		public void InitBuckPacks()
		{
			closeBuckPacksButton.onClick.AddListener(OnCloseBuckPacksButtonClicked);

			buckPacksHeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUCK_PACKS_TITLE);
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
			}

			IMetaDataModel metaDataModel = vo.storeSettingsModel;
			List<StoreItem> list = metaDataModel.store.lists["BuckPack"];
			foreach (StoreItem item in list) 
			{
				BuckPackItemPrefab thumbnail = buckPackPrefabs[item.key];

				if (item.remoteProductPrice == null) 
				{
					thumbnail.price.text = localizationService.Get(LocalizationKey.CPU_STORE_BUCK_PACKS_STORE_NOT_AVAILABLE);
				} 
				else 
				{
					thumbnail.price.text = item.remoteProductPrice;
				}
			}
		}

		public void ShowBuckPacks()
		{
			buckPacksDlg.SetActive(true);
		}

		public void HideBuckPacks()
		{
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
			buckPacksClickedSignal.Dispatch(item);
		}

		void OnCloseBuckPacksButtonClicked()
		{
			closeBuckPacksButtonClickedSignal.Dispatch();
		}

		private void CreateBuckPackPrefabs(CPUStoreVO vo)
		{
			BuckPackThumbsContainer containter = BuckPackThumbsContainer.Load();

			IMetaDataModel metaDataModel = vo.storeSettingsModel;
			BuckPackItemPrefab prefab = Instantiate<BuckPackItemPrefab>(buckPackItemPrefab);

			buckPackPrefabs = new Dictionary<string, BuckPackItemPrefab>();

			List<StoreItem> list = metaDataModel.store.lists["BuckPack"];
			foreach (StoreItem item in list) 
			{
				BuckPackItemPrefab thumbnail = Object.Instantiate(prefab);
				thumbnail.transform.SetParent(buckPacksGallery.transform, false);
				buckPackPrefabs.Add(item.key, thumbnail);

				thumbnail.bucksLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_BUCKS);
				thumbnail.payout.text = item.currency2Payout.ToString();
				thumbnail.thumbnail.sprite = containter.GetSprite(item.key);

				thumbnail.button.onClick.AddListener(() => OnBuckPackItemClicked(item));
			}

			Destroy(prefab);
		}
	}
}
