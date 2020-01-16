﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace TurboLabz.InstantFramework
{
    public class AdsSettingsModel : IAdsSettingsModel
    {
        public int slotHour { get; set; }
        public int freeNoAdsPeriod { get; set; }
        public int globalCap { get; set; }
        public int rewardedVideoCap { get; set; }
        public int interstitialCap { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            slotHour = 0;
            freeNoAdsPeriod = 0;
            globalCap = 0;
            rewardedVideoCap = 0;
            interstitialCap = 0;
        }
    }
}

