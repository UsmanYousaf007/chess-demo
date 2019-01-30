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
        [Header("Bundles Tab")]
        public GameObject[] galleryBundles;

        private IDictionary<string, SpecialBundleShopItemPrefab> prefabsBundles = null;

        public void UpdateViewBundles(StoreVO vo)
        {
            if (prefabsBundles == null)
            {
                prefabsBundles = new Dictionary<string, SpecialBundleShopItemPrefab>();
                InitPrefabsBundles(vo, galleryBundles);
            }

            foreach (KeyValuePair<string, SpecialBundleShopItemPrefab> child in prefabsBundles)
            {
                SpecialBundleShopItemPrefab bundlePrefab = child.Value;

                string textStoreNotAvailable = localizationService.Get(LocalizationKey.STORE_NOT_AVAILABLE);
                string textDays = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_DAYS);
                bundlePrefab.SetBundleState(vo, textStoreNotAvailable, textDays);
            }
        }

        private void InitPrefabsBundles(StoreVO vo, GameObject[] content)
        {
            foreach (GameObject child in content)
            {
                SpecialBundleShopItemPrefab bundlePrefab = child.GetComponent<SpecialBundleShopItemPrefab>();
                StoreItem storeItem = vo.storeSettingsModel.store.items[bundlePrefab.key];
                prefabsBundles.Add(bundlePrefab.key, bundlePrefab);
                bundlePrefab.button.onClick.AddListener(() => OnStoreItemClicked(storeItem));

                Sprite sprite = thumbsContainer.GetSprite(bundlePrefab.key);
                string textOwned = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_OWNED);
                string textRemaining = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_REMAINING);
                string textStoreNotAvailable = localizationService.Get(LocalizationKey.STORE_NOT_AVAILABLE);
                string textDays = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_DAYS);

                bundlePrefab.Populate(vo, sprite, textStoreNotAvailable, textDays, textOwned, textRemaining);
            }
        }
    }
}

