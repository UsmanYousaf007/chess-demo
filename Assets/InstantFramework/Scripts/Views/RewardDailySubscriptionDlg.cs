﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using UnityEngine;
using UnityEngine.UI;

namespace TurboLabz.InstantFramework
{
    public class RewardDailySubscriptionDlg : RewardBaseDlg
    {
        public Text headingText;
        public Image[] itemImages;
        public Text[] itemTexts;

        public void Awake()
        {
            for (int i = 0; i < itemImages.Length; i++)
            {
                itemImages[i].transform.parent.gameObject.SetActive(false);
            }
        }
    }
}
