/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.signal.impl;

namespace TurboLabz.InstantFramework
{
    public interface IRewardsService
    {
        bool dailyRewardShown { get; }
        void ShowDailyReward(string key, Signal onCloseSignal);
        void LoadDailyReward();
        void LoadDailyRewards();
    }
}
