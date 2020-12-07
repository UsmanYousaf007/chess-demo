/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
using UnityEngine;
using UnityEngine.UI;
using strange.extensions.mediation.impl;
using strange.extensions.signal.impl;
using TurboLabz.TLUtils;
using System;
using TMPro;
using System.Collections.Generic;
using System.Text;
using TurboLabz.InstantGame;
using TurboLabz.CPU;
using System.Collections;

namespace TurboLabz.InstantFramework
{
    [CLSCompliant(false)]
    public class CareerCardView : View
    {
        public TMP_Text subTitle;

        public Image nextTitleTrophy;
        public Image nextTitle;
        public Image nextTitleGlow;

        public TMP_Text nextTitleText;

        public TMP_Text trophiesLossLabel;
        public TMP_Text trophiesCountOnLosses;

        public RectTransform trophyProgressionBarFiller;
        public GameObject trophyProgressionBar;
        public TMP_Text playerTrophiesCountLabel;
        private float trophyProgressionBarOriginalWidth;

        public TMP_Text trophiesWinLabel;
        public TMP_Text trophiesCountOnWins;

        public Image bgImage;

        public TMP_Text startGame3mText;
        public Button startGame3mButton;

        public TMP_Text startGame5mText;
        public Button startGame5mButton;

        public TMP_Text startGame10mText;
        public Button startGame10mButton;

        public TMP_Text startGame30mText;
        public Button startGame30mButton;

        public void Init()
        {
            trophyProgressionBarOriginalWidth = trophyProgressionBarFiller.sizeDelta.x;
        }
    }
}
