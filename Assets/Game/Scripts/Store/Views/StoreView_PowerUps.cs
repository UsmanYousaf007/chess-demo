/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantGame
{
    public partial class StoreView : View
    {
        [Header("PowerUps Tab")]
        public GameObject scrollViewPowerUps;
        public Text titleSafeMove;
        public Text titleHint;
        public Text titleHindsight;
        public GameObject[] galleryPowerUps;

        private IDictionary<string, PowerUpShopItemPrefab> prefabsPowerUps = null;

        public void UpdateViewPowerUps(StoreVO vo)
        {
            if (prefabsPowerUps == null)
            {
                prefabsPowerUps = new Dictionary<string, PowerUpShopItemPrefab>();
                InitPrefabsPowerUps(vo, galleryPowerUps);
            }
        }

        private void InitPrefabsPowerUps(StoreVO vo, GameObject[] content)
        {
            titleSafeMove.text = localizationService.Get(LocalizationKey.STORE_POWERUP_TITLE_SAFEMOVE);
            titleHint.text = localizationService.Get(LocalizationKey.STORE_POWERUP_TITLE_HINT);
            titleHindsight.text = localizationService.Get(LocalizationKey.STORE_POWERUP_TITLE_HINDSIGHT);

            foreach (GameObject child in content)
            {
                PowerUpShopItemPrefab powerUpPrefab = child.GetComponent<PowerUpShopItemPrefab>();
                prefabsPowerUps.Add(powerUpPrefab.key, powerUpPrefab);
                StoreItem storeItem = vo.storeSettingsModel.store.items[powerUpPrefab.key];
                powerUpPrefab.button.onClick.AddListener(() => OnStoreItemClicked(storeItem));

                Sprite sprite = thumbsContainer.GetSprite(powerUpPrefab.key);
                powerUpPrefab.Populate(vo, sprite);
            }
        }
    }
}
