/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
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
                coinPrefab.price.text = storeItem.remoteProductPrice;
                //coinPrefab.payout.text = storeItem.currency2Payout.ToString();
            }
        }
    }
}

