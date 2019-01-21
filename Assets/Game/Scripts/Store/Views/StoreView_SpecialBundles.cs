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
        public GameObject[] galleryBundles;

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

            return qty + " " + displayName;
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

        private string GetBundleThemesText(StoreItem storeItem)
        {
            if (storeItem.key == GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG)
            {
                return "All Themes";
            }

            return "";
        }

        private void SetBundleStateResets(SpecialBundleShopItemPrefab bundlePrefab)
        {
            bundlePrefab.price.gameObject.SetActive(false);
            bundlePrefab.tick.gameObject.SetActive(false);
            bundlePrefab.remaining.gameObject.SetActive(false);
            bundlePrefab.remainingDays.gameObject.SetActive(false);
            bundlePrefab.discount.gameObject.SetActive(false);
            bundlePrefab.owned.gameObject.SetActive(false);
        }

        private void SetBundleStateUltimate(StoreVO vo, SpecialBundleShopItemPrefab bundlePrefab)
        {
            StoreItem storeItem = vo.storeSettingsModel.store.items[bundlePrefab.key];
            bool isOwned = vo.playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG);

            SetBundleStateResets(bundlePrefab);

            if (isOwned)
            {
                bundlePrefab.owned.gameObject.SetActive(true);
                bundlePrefab.tick.gameObject.SetActive(true);
            }
            else
            {
                bundlePrefab.price.text = storeItem.remoteProductPrice;
                bundlePrefab.discount.gameObject.SetActive(true);
            }
        }

        private void SetBundleStateStandard(StoreVO vo, SpecialBundleShopItemPrefab bundlePrefab)
        {
            StoreItem storeItem = vo.storeSettingsModel.store.items[bundlePrefab.key];
            bool isOwned = vo.playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_STANDARD_SHOP_TAG);

            SetBundleStateResets(bundlePrefab);

            if (isOwned)
            {
                string remaining = TLUtils.TimeUtil.TimeToExpireString(vo.playerModel.removeAdsTimeStamp, 30);
                remaining = remaining + " " + localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_DAYS);

                bundlePrefab.remainingDays.text = remaining;
                bundlePrefab.tick.gameObject.SetActive(true);
                bundlePrefab.remaining.gameObject.SetActive(true);
                bundlePrefab.remainingDays.gameObject.SetActive(true);
            }
            else
            {
                bundlePrefab.price.text = storeItem.remoteProductPrice;
                bundlePrefab.price.gameObject.SetActive(true);
                bundlePrefab.discount.gameObject.SetActive(true);
            }
        }

        private void SetBundleState(StoreVO vo, SpecialBundleShopItemPrefab bundlePrefab)
        {
            switch(bundlePrefab.key)
            {
                case GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG:
                    SetBundleStateUltimate(vo, bundlePrefab);
                    break;

                    case GSBackendKeys.ShopItem.SPECIAL_BUNDLE_STANDARD_SHOP_TAG:
                    SetBundleStateStandard(vo, bundlePrefab);
                    break;
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

                bundlePrefab.displayName.text = storeItem.displayName;
                bundlePrefab.thumbnail.sprite = thumbsContainer.GetSprite(bundlePrefab.key);

                bundlePrefab.payout1.text = GetBundleFeatureAdRemoveText(vo, storeItem);
                bundlePrefab.payout2.text = GetBundledItemDisplayText(vo, storeItem, GSBackendKeys.ShopItem.POWERUP_HINDSIGHT_SHOP_TAG);
                bundlePrefab.payout3.text = GetBundledItemDisplayText(vo, storeItem, GSBackendKeys.ShopItem.POWERUP_SAFEMOVE_SHOP_TAG);
                bundlePrefab.payout4.text = GetBundledItemDisplayText(vo, storeItem, GSBackendKeys.ShopItem.POWERUP_HINT_SHOP_TAG);
                bundlePrefab.payout5.text = storeItem.currency2Payout + " " + "Coins";
                bundlePrefab.payout6.text = GetBundleThemesText(storeItem);

                bundlePrefab.discount.text = GetBundleDiscountText(vo, storeItem);

                bundlePrefab.owned.text = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_OWNED);
                bundlePrefab.remaining.text = localizationService.Get(LocalizationKey.STORE_BUNDLE_FIELD_REMAINING);

                SetBundleState(vo, bundlePrefab);
            }
        }
    }
}

