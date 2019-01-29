/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public partial class StoreView : View
    {
        [Header("Coins Tab")]
        public GameObject scrollViewCoins;
        public GameObject[] galleryCoins;

        private IDictionary<string, CoinShopItemPrefab> prefabsCoins = null;

        public void UpdateViewCoins(StoreVO vo)
        {
            if (prefabsCoins == null)
            {
                prefabsCoins = new Dictionary<string, CoinShopItemPrefab>();
                InitPrefabsCoins(vo, galleryCoins);
            }

            // Update prices
            foreach (GameObject child in galleryCoins)
            {
                CoinShopItemPrefab coinPrefab = child.GetComponent<CoinShopItemPrefab>();
                StoreItem storeItem = vo.storeSettingsModel.store.items[coinPrefab.key];
                if (storeItem.remoteProductPrice == null)
                {
                    coinPrefab.price.text = localizationService.Get(LocalizationKey.STORE_NOT_AVAILABLE);
                }
                else
                {
                    coinPrefab.price.text = storeItem.remoteProductPrice;
                }
            }
        }

        private void InitPrefabsCoins(StoreVO vo, GameObject[] content)
        {
            foreach (GameObject child in content)
            {
                CoinShopItemPrefab coinPrefab = child.GetComponent<CoinShopItemPrefab>();
                StoreItem storeItem = vo.storeSettingsModel.store.items[coinPrefab.key];               

                prefabsCoins.Add(coinPrefab.key, coinPrefab);

                coinPrefab.button.onClick.AddListener(() => OnStoreItemClicked(storeItem));
                coinPrefab.displayName.text = storeItem.displayName;
                coinPrefab.thumbnail.sprite = thumbsContainer.GetSprite(coinPrefab.key);
            }
        }
    }
}

