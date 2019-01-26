/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantGame
{
    public class SpecialBundleShopItemPrefab : MonoBehaviour
    {
        public string key;

        public Image thumbnail;
        public Text displayName;
        public Text price;
        public Text discount;
        public Text owned;
        public Text remainingDays;
        public Text remaining;
        public Image tick;
        public Button button;
        public Text payout1;
        public Text payout2;
        public Text payout3;
        public Text payout4;
        public Text payout5;
        public Text payout6;

        private string GetBundledItemDisplayText(StoreVO vo, StoreItem storeItem, string itemKey)
        {
            if (!vo.storeSettingsModel.store.items.ContainsKey(itemKey))
            {
                return null;
            }

            int qty = storeItem.bundledItems[itemKey];
            string displayNameText = vo.storeSettingsModel.store.items[itemKey].displayName;

            return qty + " " + displayNameText;
        }

        private string GetBundleFeatureAdRemoveText(StoreVO vo, StoreItem storeItem)
        {
            string itemKey = GSBackendKeys.ShopItem.FEATURE_REMOVEAD_PERM_SHOP_TAG;
            if (storeItem.bundledItems.ContainsKey(itemKey))
            {
                return vo.storeSettingsModel.store.items[itemKey].displayName;
            }

            itemKey = GSBackendKeys.ShopItem.FEATURE_REMOVEAD_30_SHOP_TAG;
            if (storeItem.bundledItems.ContainsKey(itemKey))
            {
                int numDays = storeItem.bundledItems[itemKey] * 30;
                return numDays.ToString() + " Days No Ads";
                // TODO: localize or naming
                //return vo.storeSettingsModel.store.items[itemKey].displayName;
            }

            return null;
        }

        private string GetBundleDiscountText(StoreVO vo, StoreItem storeItem)
        {
            string itemKey = GSBackendKeys.ShopItem.FEATURE_REMOVEAD_PERM_SHOP_TAG;
            if (storeItem.bundledItems.ContainsKey(itemKey))
            {
                return "+50%";
            }

            itemKey = GSBackendKeys.ShopItem.FEATURE_REMOVEAD_30_SHOP_TAG;
            if (storeItem.bundledItems.ContainsKey(itemKey))
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

        private void SetBundleStateResets()
        {
            price.gameObject.SetActive(false);
            tick.gameObject.SetActive(false);
            remaining.gameObject.SetActive(false);
            remainingDays.gameObject.SetActive(false);
            discount.gameObject.SetActive(false);
            owned.gameObject.SetActive(false);
        }

        private string GetBundlePrice(StoreItem storeItem, string textStoreNotAvailable)
        {
            if (storeItem.remoteProductPrice == null)
            {
                return textStoreNotAvailable;
            }

            return storeItem.remoteProductPrice;
        }

        private void SetBundleStateUltimate(StoreVO vo, string textStoreNotAvailable)
        {
            StoreItem storeItem = vo.storeSettingsModel.store.items[key];
            bool isOwned = vo.playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG);

            SetBundleStateResets();

            if (isOwned)
            {
                owned.gameObject.SetActive(true);
                tick.gameObject.SetActive(true);
            }
            else
            {
                price.text = GetBundlePrice(storeItem, textStoreNotAvailable);
                price.gameObject.SetActive(true);
                discount.gameObject.SetActive(true);
            }
        }

        private void SetBundleStateStandard(StoreVO vo, string textStoreNotAvailable, string textDays)
        {
            StoreItem storeItem = vo.storeSettingsModel.store.items[key];
            bool isOwned = vo.playerModel.OwnsVGood(GSBackendKeys.ShopItem.SPECIAL_BUNDLE_STANDARD_SHOP_TAG);
            string remainingStr = null;

            if (isOwned)
            {
                string dayString = textDays;
                string hourString = "h";
                string minString = "m";

                remainingStr = TLUtils.TimeUtil.TimeToExpireString(vo.playerModel.removeAdsTimeStamp, vo.playerModel.removeAdsTimePeriod, minString, hourString, dayString);

                if (remainingStr == null)
                {
                    isOwned = false;
                }
            }

            SetBundleStateResets();

            if (isOwned)
            {
                remainingDays.text = remainingStr;
                tick.gameObject.SetActive(true);
                remaining.gameObject.SetActive(true);
                remainingDays.gameObject.SetActive(true);
            }
            else
            {
                price.text = GetBundlePrice(storeItem, textStoreNotAvailable);
                price.gameObject.SetActive(true);
                discount.gameObject.SetActive(true);
            }
        }

        public void SetBundleState(StoreVO vo, string textStoreNotAvailable, string textDays)
        {
            switch (key)
            {
                case GSBackendKeys.ShopItem.SPECIAL_BUNDLE_ULTIMATE_SHOP_TAG:
                    SetBundleStateUltimate(vo, textStoreNotAvailable);
                    break;

                case GSBackendKeys.ShopItem.SPECIAL_BUNDLE_STANDARD_SHOP_TAG:
                    SetBundleStateStandard(vo, textStoreNotAvailable, textDays);
                    break;
            }
        }

        public void Populate(StoreVO vo, Sprite sprite, string textStoreNotAvailable, string textDays, string textOwned, string textRemaining)
        {
            StoreItem storeItem = vo.storeSettingsModel.store.items[key];

            displayName.text = storeItem.displayName;
            if (sprite)
            {
                thumbnail.sprite = sprite;
            }
            payout1.text = GetBundleFeatureAdRemoveText(vo, storeItem);
            payout2.text = GetBundledItemDisplayText(vo, storeItem, GSBackendKeys.ShopItem.POWERUP_HINDSIGHT_SHOP_TAG);
            payout3.text = GetBundledItemDisplayText(vo, storeItem, GSBackendKeys.ShopItem.POWERUP_SAFEMOVE_SHOP_TAG);
            payout4.text = GetBundledItemDisplayText(vo, storeItem, GSBackendKeys.ShopItem.POWERUP_HINT_SHOP_TAG);
            payout5.text = storeItem.currency2Payout + " " + "Coins";
            payout6.text = GetBundleThemesText(storeItem);
            discount.text = GetBundleDiscountText(vo, storeItem);
            owned.text = textOwned;
            remaining.text = textRemaining;
        }
    }
}
