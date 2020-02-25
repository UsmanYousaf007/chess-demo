/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///

using System;

namespace TurboLabz.InstantFramework
{
    public interface IPreferencesModel
    {
        bool isAudioOn { get; set; }
        int adSlotImpressions { get; set; }    
        long adSlotId { get; set; }
        bool hasRated { get; set; }
        bool isSafeMoveOn { get; set; }
        bool isFriendScreenVisited { get; set; }
        bool isCoachTooltipShown { get; set; }
        bool isStrengthTooltipShown { get; set; }
        bool isLobbyLoadedFirstTime { get; set; }
        int coachUsedCount { get; set; }
        int strengthUsedCount { get; set; }
        int promotionCycleIndex { get; set; }
        DateTime timeAtLobbyLoadedFirstTime { get; set; }
        float timeSpentQuickMatch { get; set; }
        float timeSpentLongMatch { get; set; }
        float timeSpentCpuMatch { get; set; }
        float timeSpentLobby { get; set; }
        DateTime lastLaunchTime { get; set; }
        int globalAdsCount { get; set; }
        int rewardedAdsCount { get; set; }
        int interstitialAdsCount { get; set; }
        int resignCount { get; set; }
        bool isSkipVideoDlgShown { get; set; }
        int quickMatchStartCount { get; set; }
        int longMatchStartCount { get; set; }
        int cpuMatchStartCount { get; set; }

        void ResetDailyPrefers();
        void UpdateTimeSpentAnalyticsData(AnalyticsEventId eventId, DateTime timeAtScreenShown);

        //for appsflyer events for HUUUGE
        int videoFinishedCount { get; set; }
        int continousPlayCount { get; set; }
        int gameStartCount { get; set; }
        int gameFinishedCount { get; set; }
        DateTime appsFlyerLastLaunchTime { get; set; }
        int sessionCount { get; set; }
    }
}
