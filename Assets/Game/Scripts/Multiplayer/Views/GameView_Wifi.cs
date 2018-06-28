/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-03-17 12:37:18 UTC+05:00
/// 
/// @description
/// [add_description_here]

using UnityEngine.UI;
using TurboLabz.Chess;
using UnityEngine;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public partial class GameView
    {
        public GameObject warning;

        public void InitWifi()
        {
            // Not using for now
            // warningLabel.text = localizationService.Get(LocalizationKey.GM_WIFI_WARNING);
        }

        public void OnParentShowWifi()
        {
            HideWifiWarning();
        }

        public void WifiHealthUpdate(bool isHealthy)
        {
            if (isHealthy)
            {
                HideWifiWarning();
            }
            else
            {
                ShowWifiWarning();
            }
        }

        void ShowWifiWarning()
        {
            warning.SetActive(true);
        }

        void HideWifiWarning()
        {
            warning.SetActive(false);
        }

    }
}
