/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace TurboLabz.InstantFramework
{
    public interface IRewardsSettingsModel
    {
        int matchWinReward { get; set; }          // Winner coins reward
        int matchWinAdReward { get; set; }        // Additional Winner coins reward on Ad watch
        int matchRunnerUpReward { get; set; }     // Loser coins reward 
        int matchRunnerUpAdReward { get; set; }   // Additional Loser coins reward on Ad watch
        int failSafeCoinReward { get; set; }
        int facebookConnectReward { get; set; }
        int powerUpCoinsValue { get; set; }
        int ratingBoostReward { get; set; }

        float coefficientWinVideo { get; set; }
        float coefficientWinIntersitial { get; set; }
        float coefficientLoseVideo { get; set; }
        float coefficientLoseIntersitial { get; set; }

        int getRewardCoins(AdType adType, int powerUpUsage, bool playerWins);
    }
}

