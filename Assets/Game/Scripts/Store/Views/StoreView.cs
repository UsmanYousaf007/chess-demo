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
    public partial class StoreView : View
    {
		[Inject] public ILocalizationService localizationService { get; set; }

        public Text title;
		public GameObject gallery;
		public SkinShopItemPrefab skinShopItemPrefab;

		// View signals
		public Signal<StoreItem> skinItemClickedSignal = new Signal<StoreItem>();

		List<SkinShopItemPrefab> prefabs = null;

        // Current selected item
        public string currentSkinItemId;

        // Original selected item
        public string originalSkinItemId;

        public void Init()
        {
            title.text = localizationService.Get(LocalizationKey.CPU_STORE_HEADING);
        }

		public void UpdateView(StoreVO vo)
		{
            // Set original selected item
            originalSkinItemId = vo.playerModel.activeSkinId;
            currentSkinItemId = originalSkinItemId;

            if (prefabs == null) 
			{
				CreatePrefabs(vo);	
			}

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


		public bool IsVisible()
		{
			return gameObject.activeSelf;
		}

        public bool HasSkinChanged()
        {
            return currentSkinItemId != originalSkinItemId;
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
                skinThumbnail.tick.gameObject.SetActive(false);
                skinThumbnail.itemId = item.key;
			}

            Destroy(prefab.gameObject);
		}

        private void SetItemOwned(SkinShopItemPrefab thumbnail)
        {
            Color colorOwned = Colors.YELLOW;
            thumbnail.price.text = localizationService.Get(LocalizationKey.CPU_STORE_OWNED);
            thumbnail.price.color = colorOwned;
            thumbnail.bucksIcon.gameObject.SetActive (false);
        }

        private void SetItemForPurchase(SkinShopItemPrefab thumbnail, int cost)
        {
            Color colorNormal = Colors.WHITE;
            thumbnail.price.text = cost.ToString();
            thumbnail.price.color = colorNormal;
            thumbnail.bucksIcon.gameObject.SetActive (true);
        }

		private void PopulateSkins(StoreVO vo)
		{
            TLUtils.LogUtil.Log("PopulateSkins() ", "cyan");

            IMetaDataModel metaDataModel = vo.storeSettingsModel;
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
                    SetItemOwned(skinThumbnail);
                    if (vo.playerModel.activeSkinId == item.key)
                    {
                        skinThumbnail.tick.gameObject.SetActive(true);
                    }
				} 
				else 
				{
                    SetItemForPurchase(skinThumbnail, item.currency2Cost);
				}
			}
		}

        private SkinShopItemPrefab GetThumbnailPrefab(string shopItemId)
        {
            bool found = false;
            int i = 0;

            while (!found && i < prefabs.Count) 
            {
                found = prefabs[i].itemId == shopItemId;
                if (!found)
                {
                    i++;
                }
            }

            return found ? prefabs[i] : null;          
        }

        private void ClearThumbailTicks()
        {
            for (int i = 0; i < prefabs.Count; i++) 
            {
                prefabs[i].tick.gameObject.SetActive(false);
            }
        }

        public void UpdateItemThumbnail(string shopItemId)
        {
            SkinShopItemPrefab thumnail = GetThumbnailPrefab(shopItemId);
            if (thumnail != null)
            {
                SetItemOwned(thumnail);

                if (currentSkinItemId == shopItemId)
                {
                    ClearThumbailTicks();
                    thumnail.tick.gameObject.SetActive(true);
                }
            }
        }

		private void OnSkinItemClicked(StoreItem item)
		{
			skinItemClickedSignal.Dispatch(item);
		}
	}
}
