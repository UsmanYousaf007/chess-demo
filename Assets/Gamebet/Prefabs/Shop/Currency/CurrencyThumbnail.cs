/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-13 09:54:17 UTC+05:00
using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.Gamebet
{
    public class CurrencyThumbnail : MonoBehaviour
    {
        public Button thumbnailButton;
        public Image background;
        public Text bucksPrice;
        public Text iapPrice;

        public Image imageFrame;
        public Text title;
        public Text rewardWithBonus;
        public Image icon;
        public Text rewardWithoutBonus;
        public Text bonus;

        public GameObject bucksAmout;
        public GameObject iapAmout;
    }
}
