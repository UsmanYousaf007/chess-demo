/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl ;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using System.Collections.Generic;

namespace TurboLabz.InstantChess
{
    public partial class CPUStoreView : View
    {
		[Inject] public ILocalizationService localizationService { get; set; }

		public Text playerBucks;
		public GameObject gallery;
		public Button backButton;
		public Button addBucksButton;
		public SkinShopItemPrefab skinShopItemPrefab;

		// View signals
		public Signal backButtonClickedSignal = new Signal();
		public Signal<StoreItem> skinItemClickedSignal = new Signal<StoreItem>();
		public Signal addBucksButtonClickedSignal = new Signal();

		List<SkinShopItemPrefab> prefabs = null;

        public void Init()
        {
			backButton.onClick.AddListener(OnBackButtonClicked);
			addBucksButton.onClick.AddListener(OnAddBucksButtonClicked);
        }

		public void UpdateView(CPUStoreVO vo)
		{
			if (prefabs == null) 
			{
				CreatePrefabs(vo);	
			}

			playerBucks.text = vo.playerModel.bucks.ToString();
			PopulateSkins(vo);
		}

		public void Show() 
		{ 
			gameObject.SetActive(true); 
		}

		public void Hide()
		{ 
			gameObject.SetActive(false); 
		}

		private void OnBackButtonClicked()
		{
			backButtonClickedSignal.Dispatch();
		}

		private void CreatePrefabs(CPUStoreVO vo)
		{
            IMetaDataModel metaDataModel = vo.storeSettingsModel;
			SkinShopItemPrefab prefab = Instantiate<SkinShopItemPrefab>(skinShopItemPrefab);

			prefabs = new List<SkinShopItemPrefab>();

			List<StoreItem> list = metaDataModel.lists["Skin"];
			foreach (StoreItem item in list) 
			{
				SkinShopItemPrefab skinThumbnail = Object.Instantiate(prefab);
				skinThumbnail.transform.SetParent(gallery.transform, false);
				prefabs.Add (skinThumbnail);
			}

			Destroy(prefab);
		}

		private void PopulateSkins(CPUStoreVO vo)
		{
            IMetaDataModel metaDataModel = vo.storeSettingsModel;

			List<StoreItem> list = metaDataModel.lists["Skin"];
			for (int i = 0; i < list.Count; i++) 
			{
				SkinShopItemPrefab skinThumbnail = prefabs[i];
				StoreItem item = list[i];

				skinThumbnail.displayName.text = item.displayName;
				if (vo.playerModel.ownsVGood (item.key)) 
				{
					skinThumbnail.price.text = "Owned";
					skinThumbnail.bucksIcon.gameObject.SetActive (false);
				} 
				else 
				{
					skinThumbnail.price.text = item.currency2Cost.ToString();
					skinThumbnail.bucksIcon.gameObject.SetActive (true);
				}

				skinThumbnail.button.onClick.AddListener(() => OnSkinItemClicked(item));
			}
		}

		private void OnSkinItemClicked(StoreItem item)
		{
			skinItemClickedSignal.Dispatch(item);
		}

		private void OnAddBucksButtonClicked()
		{
			addBucksButtonClickedSignal.Dispatch();
		}
	}
}
