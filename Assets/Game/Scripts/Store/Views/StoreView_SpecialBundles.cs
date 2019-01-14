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
        [Header("Bundles Tab")]
        public GameObject galleryBundles;

        private IDictionary<string, SpecialBundleShopItemPrefab> prefabsBundles = null;

        public void UpdateViewBundles(StoreVO vo)
        {
            if (prefabsBundles == null)
            {
                prefabsBundles = new Dictionary<string, SpecialBundleShopItemPrefab>();
                InitPrefabsBundles(vo, galleryBundles);
            }
        }

        private string GetBundledItemDisplayText(StoreVO vo, StoreItem storeItem, string itemKey)
        {
            if (!vo.storeSettingsModel.store.items.ContainsKey(itemKey))
            {
                return null;
            }

            int qty = storeItem.bundledItems[itemKey];
            string displayName = vo.storeSettingsModel.store.items[itemKey].displayName;

            return displayName + " " + "x" + qty;
        }

        private string GetBundleFeatureAdRemoveText(StoreVO vo, StoreItem storeItem)
        {
            string key = GSBackendKeys.ShopItem.FEATURE_REMOVEAD_PERM_SHOP_TAG;
            if (storeItem.bundledItems.ContainsKey(key))
            {
                return vo.storeSettingsModel.store.items[key].displayName;
            }

            key = GSBackendKeys.ShopItem.FEATURE_REMOVEAD_30_SHOP_TAG;
            if (storeItem.bundledItems.ContainsKey(key))
            {
                return vo.storeSettingsModel.store.items[key].displayName;
            }

            return null;
        }

        private string GetBundleDiscountText(StoreVO vo, StoreItem storeItem)
        {
            string key = GSBackendKeys.ShopItem.FEATURE_REMOVEAD_PERM_SHOP_TAG;
            if (storeItem.bundledItems.ContainsKey(key))
            {
                return "+50%";
            }

            key = GSBackendKeys.ShopItem.FEATURE_REMOVEAD_30_SHOP_TAG;
            if (storeItem.bundledItems.ContainsKey(key))
            {
                return "+40%";
            }

            return null;
        }

        private void InitPrefabsBundles(StoreVO vo, GameObject content)
        {
            foreach (Transform child in content.transform)
            {
                SpecialBundleShopItemPrefab bundlePrefab = child.GetComponent<SpecialBundleShopItemPrefab>();
                StoreItem storeItem = vo.storeSettingsModel.store.items[bundlePrefab.key];

                prefabsBundles.Add(bundlePrefab.key, bundlePrefab);

                bundlePrefab.youEnjoy.text = localizationService.Get(LocalizationKey.STORE_BUNDLE_YOU_ENJOY);
                bundlePrefab.button.onClick.AddListener(() => OnStoreItemClicked(storeItem));

                bundlePrefab.displayName.text = storeItem.displayName;
                bundlePrefab.thumbnail.sprite = thumbsContainer.GetSprite(bundlePrefab.key);
                bundlePrefab.price.text = storeItem.remoteProductPrice;

                bundlePrefab.payout1.text = GetBundleFeatureAdRemoveText(vo, storeItem);
                bundlePrefab.payout2.text = GetBundledItemDisplayText(vo, storeItem, GSBackendKeys.ShopItem.POWERUP_HINDSIGHT_SHOP_TAG);
                bundlePrefab.payout3.text = GetBundledItemDisplayText(vo, storeItem, GSBackendKeys.ShopItem.POWERUP_SAFEMOVE_SHOP_TAG);
                bundlePrefab.payout4.text = GetBundledItemDisplayText(vo, storeItem, GSBackendKeys.ShopItem.POWERUP_HINT_SHOP_TAG);
                bundlePrefab.payout5.text = "Coins x" + storeItem.currency2Payout;
                bundlePrefab.payout6.text = ""; // TODO: Exclusive Skins

                bundlePrefab.discount.text = GetBundleDiscountText(vo, storeItem);
            }
        }
    }
}

