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
        public int facebookConnectReward { get; set; }
        public int failSafeCoinReward { get; set; }
        public int powerUpCoinsValue { get; set; }

        public float coefficientWinVideo { get; set; }
        public float coefficientWinIntersitial { get; set; }
        public float coefficientLoseVideo { get; set; }
        public float coefficientLoseIntersitial { get; set; }


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
            facebookConnectReward = 10;
            failSafeCoinReward = 10;
            coefficientWinVideo = 0.5f;
            coefficientWinIntersitial = 0.3f;
            coefficientLoseVideo = 0.4f;
            coefficientLoseIntersitial = 0.2f;
            powerUpCoinsValue = 1;
        }

        public int getRewardCoins(AdType adType, int powerUpUsage, bool playerWins)
        {
            float rewardCoins = 0.0f;
            float coefficient = 0.1f;

            if (adType == AdType.Interstitial)
            {
                if (playerWins)
                {
                    coefficient = coefficientWinIntersitial;
                }
                else
                {
                    coefficient = coefficientLoseIntersitial;
                }

            }
            else if (adType == AdType.RewardedVideo)
            {

                if (playerWins)
                {
                    coefficient = coefficientWinVideo;
                }
                else
                {
                    coefficient = coefficientLoseVideo;
                }
            }

            if(powerUpUsage > 0)
            {
                rewardCoins = coefficient * powerUpUsage * powerUpCoinsValue;
            }
            else
            {
                rewardCoins =  coefficient * failSafeCoinReward;
            }

            int returnCoins = (int)Math.Ceiling(rewardCoins);

            return returnCoins;
        }
    }
}

