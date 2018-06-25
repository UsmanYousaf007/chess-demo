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

namespace TurboLabz.InstantGame
{
    public partial class CPUStoreView : View
    {
		[Inject] public ILocalizationService localizationService { get; set; }

		public Text HeadingLabel;
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

			HeadingLabel.text = localizationService.Get(LocalizationKey.CPU_STORE_HEADING);
        }

		public void UpdateView(StoreVO vo)
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

		public void UpdatePlayerBucks(long bucks)
		{
			playerBucks.text = bucks.ToString();
		}

		public bool IsVisible()
		{
			return gameObject.activeSelf;
		}

		private void OnBackButtonClicked()
		{
			backButtonClickedSignal.Dispatch();
		}

		private void CreatePrefabs(StoreVO vo)
		{
			SkinThumbsContainer containter = SkinThumbsContainer.Load();

            IMetaDataModel metaDataModel = vo.storeSettingsModel;
			SkinShopItemPrefab prefab = Instantiate<SkinShopItemPrefab>(skinShopItemPrefab);

			prefabs = new List<SkinShopItemPrefab>();

			List<StoreItem> list = metaDataModel.store.lists["Skin"];
			foreach (StoreItem item in list) 
			{
				if (item.state == StoreItem.State.DISABLED) 
				{
					continue;
				}

				SkinShopItemPrefab skinThumbnail = Object.Instantiate(prefab);
				skinThumbnail.transform.SetParent(gallery.transform, false);
				prefabs.Add (skinThumbnail);

				skinThumbnail.button.onClick.AddListener(() => OnSkinItemClicked(item));
				skinThumbnail.displayName.text = item.displayName;
				skinThumbnail.thumbnail.sprite = containter.GetSprite(item.key);
			}

			Destroy(prefab);
		}

		private void PopulateSkins(StoreVO vo)
		{
            
            IMetaDataModel metaDataModel = vo.storeSettingsModel;
            Color colorOwned = Colors.YELLOW;
            Color colorNormal = Colors.WHITE;

			int i = 0;
			List<StoreItem> list = metaDataModel.store.lists["Skin"];
			foreach (StoreItem item in list) 
			{
				if (item.state == StoreItem.State.DISABLED) 
				{
					continue;
				}

				SkinShopItemPrefab skinThumbnail = prefabs[i++];

                if (vo.playerModel.OwnsVGood(item.key)) 
				{
					skinThumbnail.price.text = localizationService.Get(LocalizationKey.CPU_STORE_OWNED);
					skinThumbnail.price.color = colorOwned;
					skinThumbnail.bucksIcon.gameObject.SetActive (false);
				} 
				else 
				{
					skinThumbnail.price.text = item.currency2Cost.ToString();
					skinThumbnail.price.color = colorNormal;
					skinThumbnail.bucksIcon.gameObject.SetActive (true);
				}
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
