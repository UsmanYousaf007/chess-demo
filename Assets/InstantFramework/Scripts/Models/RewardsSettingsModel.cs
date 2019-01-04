/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace TurboLabz.InstantFramework
{
    public class RewardsSettingsModel : IRewardsSettingsModel
    {
        public int matchWinReward { get; set; }
        public int matchWinAdReward { get; set; }
        public int matchRunnerUpReward { get; set; }
        public int matchRunnerUpAdReward { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            matchWinReward = 0;
            matchWinAdReward = 0;
            matchRunnerUpReward = 0;
            matchRunnerUpAdReward = 0;
        }
    }
}

