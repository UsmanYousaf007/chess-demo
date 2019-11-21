using System;

namespace TurboLabz.InstantFramework
{
    public class SettingsModel : ISettingsModel
    {
        public int maxLongMatchCount { get; set; }
        public int maxFriendsCount { get; set; }
        public int facebookConnectReward { get; set; }
        public int maxRecentlyCompletedMatchCount { get; set; }

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
        }
    }
}