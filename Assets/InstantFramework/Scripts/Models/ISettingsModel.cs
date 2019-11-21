/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

namespace TurboLabz.InstantFramework
{
    public interface ISettingsModel
    {
        int maxLongMatchCount { get; set; }
        int maxFriendsCount { get; set; }
        int facebookConnectReward { get; set; }
        int maxRecentlyCompletedMatchCount { get; set; }


    }
}
