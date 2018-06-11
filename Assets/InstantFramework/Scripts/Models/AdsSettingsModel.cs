﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace TurboLabz.InstantFramework
{
    public class AdsSettingsModel : IAdsSettingsModel
    {
        public int maxImpressionsPerSlot { get; set; }
        public int slotMinutes { get; set; }
        public int adsRewardIncrement { get; set; }

        public void Reset()
        {
            maxImpressionsPerSlot = 0;
            slotMinutes = 0;
            adsRewardIncrement = 0;
        }
    }
}

