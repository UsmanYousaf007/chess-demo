/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

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
    }
}
