/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class RewardTournamentEndDlg : RewardBaseDlg
    {
        public Text headingText;

        public Text subHeadtingLabel;
        public Text rankCountText;

        public Text trophiesCountText;
        public Text trophiesAlreadyAdded;

        public Image chestImage;
        public Text chestText;

        public Image[] itemImages;
        public Text[] itemTexts;

        public GameObject chestSection;
        public GameObject rewardsSection;

        public RectTransform layout;

        public void Awake()
        {
            for (int i = 0; i < itemImages.Length; i++)
            {
                itemImages[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
