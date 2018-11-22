/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace TurboLabz.InstantFramework
{
    public class AdsSettingsModel : IAdsSettingsModel
    {
        public int maxImpressionsPerSlot { get; set; }
        public int slotHour { get; set; }
        public int adsRewardIncrement { get; set; }
        public int freeNoAdsPeriod { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            maxImpressionsPerSlot = 0;
            slotHour = 0;
            adsRewardIncrement = 0;
            freeNoAdsPeriod = 0;
        }
    }
}

