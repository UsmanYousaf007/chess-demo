/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public interface ISettingsModel
    {
        int maxLongMatchCount { get; set; }
        int maxFriendsCount { get; set; }
        int facebookConnectReward { get; set; }
        int maxRecentlyCompletedMatchCount { get; set; }

        int maxCommunityMatches { get; set; }
        bool maintenanceFlag { get; set; }
        bool appUpdateFlag { get; set; }
        string updateMessage { get; set; }
        string maintenanceMessage { get; set; }
        string minimumClientVersion { get; set; }

        bool maintenanceWarningFlag { get; set; }
        string maintenanceWarningMessege { get; set; }
        string maintenanceWarningBgColor { get; set; }
        string updateReleaseBannerMessage { get; set; }

        string manageSubscriptionURL { get; set; }
        int maxLongMatchCountPremium { get; set; }
        int maxFriendsCountPremium { get; set; }
        int hintsAllowedPerGame { get; set; }
        int dailyNotificationDeadlineHour { get; set; }
        string defaultSubscriptionKey { get; set; }

        int matchmakingRandomRange { get; set; }

        Dictionary<string, int> inventorySpecialItemsRewardedVideoCost { get; set; }
        int GetInventorySpecialItemsRewardedVideoCost(string key);
    }
}
