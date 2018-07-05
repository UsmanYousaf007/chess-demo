/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-09-19 18:54:42 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;

using strange.extensions.mediation.impl;
using UnityEngine;
using System.Collections;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantFramework
{
    public class BottomNavView : View
    {
        [Inject] public ILocalizationService localizationService { get; set; }

        public Text homeLabel;
        public Text profileLabel;
        public Text shopLabel;
        public Text settingsLabel;

        public void Init()
        {
            homeLabel.text = localizationService.Get(LocalizationKey.NAV_HOME);
            profileLabel.text = localizationService.Get(LocalizationKey.NAV_PROFILE);
            shopLabel.text = localizationService.Get(LocalizationKey.NAV_SHOP);
            settingsLabel.text = localizationService.Get(LocalizationKey.NAV_SETTINGS);
        }
    }
}
