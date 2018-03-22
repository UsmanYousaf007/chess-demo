/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author IRTAZA MUMTAZ <irtaza.mumtaz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-02-20 18:09:45 UTC+05:00

using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.Gamebet
{
    public class LootBoxesThumbnail : MonoBehaviour
    {
        public Button thumbnailButton;
        public Image frame;
        public Image background;
        public Text nameLabel;
        public Text amountLabel;
        public Text ownedLabel;
        public Image image;

        public GameObject owned;
        public GameObject price;
    }
}
