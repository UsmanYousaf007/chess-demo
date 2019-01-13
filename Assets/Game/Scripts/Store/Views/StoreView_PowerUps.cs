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
        [Header("PowerUps Tab")]
        public GameObject scrollViewPowerUps;
        public GameObject galleryPowerUps;

        private IDictionary<string, PowerUpShopItemPrefab> prefabsPowerUps = null;

        public void UpdateViewPowerUps(StoreVO vo)
        {
            if (prefabsPowerUps == null)
            {
                prefabsPowerUps = new Dictionary<string, PowerUpShopItemPrefab>();
                InitPrefabsPowerUps(vo, galleryPowerUps);
            }
        }

        private void InitPrefabsPowerUps(StoreVO vo, GameObject content)
        {
            foreach (Transform child in content.transform)
            {
                PowerUpShopItemPrefab powerUpPrefab = child.GetComponent<PowerUpShopItemPrefab>();
                StoreItem storeItem = vo.storeSettingsModel.store.items[powerUpPrefab.key];

                prefabsPowerUps.Add(powerUpPrefab.key, powerUpPrefab);

                powerUpPrefab.button.onClick.AddListener(() => OnStoreItemClicked(storeItem));
                powerUpPrefab.displayName.text = storeItem.displayName;
                powerUpPrefab.thumbnail.sprite = thumbsContainer.GetSprite(powerUpPrefab.key);
                powerUpPrefab.price.text = storeItem.currency2Cost.ToString();
            }
        }
    }
}
