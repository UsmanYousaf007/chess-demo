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

        public Dictionary<string, int> inventorySpecialItemsRewardedVideoCost { get; set; }

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

            inventorySpecialItemsRewardedVideoCost = new Dictionary<string, int>();
        }

        public int GetInventorySpecialItemsRewardedVideoCost(string key)
        {
            return inventorySpecialItemsRewardedVideoCost.ContainsKey(key) ? inventorySpecialItemsRewardedVideoCost[key] : 0;
        }
    }
}