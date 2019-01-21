/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantGame
{
    public class TopInventoryBarPrefab : MonoBehaviour
    {
        public Button safeMoveButton;
        public Button hintButton;
        public Button hindsightButton;
        public Button infoButton;
        public Button addCoinsButton;

        public TextMeshProUGUI safeMoveCountText;
        public TextMeshProUGUI hintCountText;
        public TextMeshProUGUI hindsightCountText;
        public Text coinsCountText;

        public Image safeMovePlus;
        public Image hintPlus;
        public Image hindsightPlus;
    }
}
