using System;
using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class SettingsModel : ISettingsModel
    {
        public int maxLongMatchCount { get; set; }
        public int maxFriendsCount { get; set; }
        public int facebookConnectReward { get; set; }
        public int maxRecentlyCompletedMatchCount { get; set; }

        public int maxCommunityMatches { get; set; }
        public bool maintenanceFlag { get; set; }
        public bool appUpdateFlag { get; set; }
        public string updateMessage { get; set; }
        public string maintenanceMessage { get; set; }
        public string minimumClientVersion { get; set; }
        public bool maintenanceWarningFlag { get; set; }
        public string maintenanceWarningMessege { get; set; }
        public string maintenanceWarningBgColor { get; set; }
        public string updateReleaseBannerMessage { get; set; }
        public string manageSubscriptionURL { get; set; }
        public int maxLongMatchCountPremium { get; set; }
        public int maxFriendsCountPremium { get; set; }
        public int hintsAllowedPerGame { get; set; }
        public int dailyNotificationDeadlineHour { get; set; }
        public string defaultSubscriptionKey { get; set; }
        public int matchmakingRandomRange { get; set; }
        public long allStarLeaderboardLastFetchTime { get; set; }

        public Dictionary<string, int> inventorySpecialItemsRewardedVideoCost { get; set; }
        public List<long> bettingIncrements { get; set; }
        public List<float> defaultBetIncrementByGamesPlayed { get; set; }

        public int advantageThreshold { get; set; }
        public int purchasedHintsThreshold { get; set; }
        public int powerModeFreeHints { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
        }

        private void Reset()
        {
            maxLongMatchCount = 2;
            maxFriendsCount = 2;
            facebookConnectReward = 10;
            maxRecentlyCompletedMatchCount = 10;

            maxCommunityMatches = 6;
            maintenanceFlag = false;
            appUpdateFlag = false;
            updateMessage = "";
            maintenanceMessage = "";
            minimumClientVersion = "";
            maintenanceWarningBgColor = "";
            updateReleaseBannerMessage = "";
            manageSubscriptionURL = "";
            maxLongMatchCountPremium = 0;
            maxFriendsCountPremium = 0;
            hintsAllowedPerGame = 0;
            dailyNotificationDeadlineHour = 0;
            defaultSubscriptionKey = "Subscription";
            matchmakingRandomRange = 0;

            inventorySpecialItemsRewardedVideoCost = new Dictionary<string, int>();
            bettingIncrements = new List<long>();
            defaultBetIncrementByGamesPlayed = new List<float>();
            advantageThreshold = 0;
            purchasedHintsThreshold = 0;
            powerModeFreeHints = 0;
        }

        public int GetInventorySpecialItemsRewardedVideoCost(string key)
        {
            return inventorySpecialItemsRewardedVideoCost.ContainsKey(key) ? inventorySpecialItemsRewardedVideoCost[key] : 0;
        }
    }
}