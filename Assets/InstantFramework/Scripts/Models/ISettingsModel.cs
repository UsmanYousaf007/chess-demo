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
        long allStarLeaderboardLastFetchTime { get; set; }

        int opponentHigherEloCap { get; set; }
        int opponentLowerEloCapMin { get; set; }
        int opponentLowerEloCapMax { get; set; }

        Dictionary<string, int> inventorySpecialItemsRewardedVideoCost { get; set; }
        int GetInventorySpecialItemsRewardedVideoCost(string key);

        List<long> bettingIncrements { get; set; }
        List<float> defaultBetIncrementByGamesPlayed { get; set; }
        Dictionary<string, float> matchCoinsMultiplayer { get; set; }

        int advantageThreshold { get; set; }
        int purchasedHintsThreshold { get; set; }
        int powerModeFreeHints { get; set; }

        bool isHuuugeServerValidationEnabled { get; set; }
        long maintenanceWarningTimeStamp { get; set; }

        float GetSafeCoinsMultiplyer(string key);

        int sessionDurationForGDPRinMinutes { get; set; }
    }
}
