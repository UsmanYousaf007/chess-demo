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

        public TMP_Text startGameText;
        public Button startGameButton;

        public Image nextTitleTrophy;
        public Image nextTitle;
        public Image nextTitleGlow;

        public TMP_Text nextTitleText;
        public TMP_Text neededTrophiesCount;

        public void Init()
        {

        }
    }
}
