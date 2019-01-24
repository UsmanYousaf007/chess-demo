/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using TurboLabz.InstantFramework;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantGame
{
    public class PowerUpShopItemPrefab : MonoBehaviour
    {
        public string key;

        public Image thumbnail;
        public Text displayName;
        public Text price;
        public Button button;
        public Image bucksIcon;
        public Text quantity;

        public void Populate(StoreVO vo, Sprite sprite)
        {
            StoreItem storeItem = vo.storeSettingsModel.store.items[key];
            displayName.text = GetPowerUpBundledItemDisplayText(vo, storeItem);
            thumbnail.sprite = sprite;
            price.text = storeItem.currency2Cost.ToString();
            quantity.text = GetPowerUpBundledItemQuantityText(storeItem);
        }

        private string GetPowerUpBundledItemDisplayText(StoreVO vo, StoreItem storeItem)
        {
            var e = storeItem.bundledItems.GetEnumerator();
            e.MoveNext();
            KeyValuePair<string, int> val = e.Current;
            StoreItem bundleStoreItem = vo.storeSettingsModel.store.items[val.Key];

            return bundleStoreItem.displayName;
        }

        private string GetPowerUpBundledItemQuantityText(StoreItem storeItem)
        {
            var e = storeItem.bundledItems.GetEnumerator();
            e.MoveNext();
            KeyValuePair<string, int> val = e.Current;

            return "x" + val.Value.ToString();
        }

    }
}
