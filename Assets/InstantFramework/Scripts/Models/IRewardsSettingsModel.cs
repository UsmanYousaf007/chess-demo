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
    }
}

