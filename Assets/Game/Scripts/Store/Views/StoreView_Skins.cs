/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using System.Collections.Generic;
using strange.extensions.signal.impl;
using TurboLabz.InstantFramework;
using strange.extensions.mediation.impl;

namespace TurboLabz.InstantGame
{
    public partial class StoreView : View
    {
        [Header("Skins Tab")]
        public GameObject scrollViewSkins;
        public GameObject[] gallerySkins;

        private IDictionary<string, SkinShopItemPrefab> prefabsSkins = null;

        [HideInInspector] public string currentSkinItemId;
        [HideInInspector] public string originalSkinItemId;

        public void UpdateViewSkins(StoreVO vo)
        {
            originalSkinItemId = vo.playerModel.activeSkinId;
            currentSkinItemId = originalSkinItemId;

            if (prefabsSkins == null)
            {
                prefabsSkins = new Dictionary<string, SkinShopItemPrefab>();
                InitPrefabsSkins(vo, gallerySkins);
            }

            UpdateSkins(vo);
        }

        private void InitPrefabsSkins(StoreVO vo, GameObject[] content)
        {
            foreach (GameObject child in content)
            {
                SkinShopItemPrefab skinPrefab = child.GetComponent<SkinShopItemPrefab>();
                StoreItem sourceItem = vo.storeSettingsModel.store.items[skinPrefab.key];
                StoreItem sourceSkinItem = sourceItem as StoreItem;

                prefabsSkins.Add(skinPrefab.key, skinPrefab);

                skinPrefab.button.onClick.AddListener(() => OnStoreItemClicked(sourceSkinItem));
                skinPrefab.displayName.text = sourceItem.displayName;
                skinPrefab.thumbnail.sprite = thumbsContainer.GetSprite(skinPrefab.key);
                skinPrefab.tick.gameObject.SetActive(false);
            }
        }

        private void UpdateSkins(StoreVO vo)
        {
            foreach (KeyValuePair<string, SkinShopItemPrefab> v in prefabsSkins)
            {
                SkinShopItemPrefab skinPrefab = v.Value;

                if (vo.playerModel.OwnsVGood(skinPrefab.key))
                {
                    SetItemOwned(skinPrefab);

                    if (vo.playerModel.activeSkinId == skinPrefab.key)
                    {
                        skinPrefab.tick.gameObject.SetActive(true);
                    }
                }
                else
                {
                    int price = vo.storeSettingsModel.store.items[skinPrefab.key].currency2Cost;
                    SetItemForPurchase(skinPrefab, price);
                }
            }
        }

        public bool HasSkinChanged()
        {
            return currentSkinItemId != originalSkinItemId;
        }

        private void SetItemOwned(SkinShopItemPrefab thumbnail)
        {
            thumbnail.price.text = localizationService.Get(LocalizationKey.CPU_STORE_OWNED);
            thumbnail.bucksIcon.gameObject.SetActive(false);
        }

        private void SetItemForPurchase(SkinShopItemPrefab thumbnail, int cost)
        {
            thumbnail.price.text = cost.ToString();
            thumbnail.bucksIcon.gameObject.SetActive(true);
        }

        private void ClearThumbailTicks()
        {
            foreach (KeyValuePair<string, SkinShopItemPrefab> v in prefabsSkins)
            {
                SkinShopItemPrefab skinPrefab = v.Value;
                skinPrefab.tick.gameObject.SetActive(false);
            }
        }

        public void UpdateItemThumbnail(string shopItemId)
        {
            SkinShopItemPrefab skinPrefab = prefabsSkins[shopItemId];
            if (skinPrefab != null)
            {
                SetItemOwned(skinPrefab);

                if (currentSkinItemId == shopItemId)
                {
                    ClearThumbailTicks();
                    skinPrefab.tick.gameObject.SetActive(true);
                }
            }
        }
    }
}
