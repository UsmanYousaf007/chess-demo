/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-21 10:43:27 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine.UI;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using UnityEngine;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        [Header("Bot Bar")]
        public Text company;
        public Text prev;
        public Text next;
        public Button prevButton;
        public Button nextButton;

        public void OnParentShowBotBar()
        {
            company.text = localizationService.Get(LocalizationKey.BOT_NAV_COMPANY);
            prev.text = localizationService.Get(LocalizationKey.BOT_NAV_PREV);
            next.text = localizationService.Get(LocalizationKey.BOT_NAV_NEXT);
        }

        public void UpdateBotBar()
        {
        }
    }
}
