using System;

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
        public string updateMessage { get; set; }
        public string maintenanceMessage { get; set; }
        public string minimumClientVersion { get; set; }

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
            updateMessage = "";
            maintenanceMessage = "";
            minimumClientVersion = "";
        }
    }
}