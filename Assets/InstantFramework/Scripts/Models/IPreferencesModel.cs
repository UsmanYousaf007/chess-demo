/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///

using System;
using System.Collections.Generic;

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
        float timeSpent1mMatch { get; set; }
        float timeSpent3mMatch { get; set; }
        float timeSpent5mMatch { get; set; }
        float timeSpent10mMatch { get; set; }
        float timeSpent30mMatch { get; set; }
        float timeSpentLongMatch { get; set; }
        float timeSpentCpuMatch { get; set; }
        float timeSpent1mTournament { get; set; }
        float timeSpent3mTournament { get; set; }
        float timeSpent5mTournament { get; set; }
        float timeSpent10mTournament { get; set; }
        DateTime lastLaunchTime { get; set; }
        int globalAdsCount { get; set; }
        int rewardedAdsCount { get; set; }
        int interstitialAdsCount { get; set; }
        int resignCount { get; set; }
        bool isSkipVideoDlgShown { get; set; }
        DateTime timeAtSubscrptionDlgShown { get; set; }
        int autoSubscriptionDlgShownCount { get; set; }
        int rankedMatchesFinishedCount { get; set; }
        bool isAutoSubsriptionDlgShownFirstTime { get; set; }
        bool isFirstRankedGameOfTheDayFinished { get; set; }
        bool isInstallDayOver { get; set; }
        int installDayGameCount { get; set; }
        string installDayFavMode { get; set; }
        string overallFavMode { get; set; }
        int favModeCount { get; set; }
        int gameCount1m { get; set; }
        int gameCount3m { get; set; }
        int gameCount5m { get; set; }
        int gameCount10m { get; set; }
        int gameCount30m { get; set; }
        int gameCountLong { get; set; }
        int gameCountCPU { get; set; }
        bool isAllLessonsCompleted { get; set; }
        int cpuPowerUpsUsedCount { get; set; }
        bool inventoryTabVisited { get; set; }
        bool shopTabVisited { get; set; }
        bool themesTabVisited { get; set; }
        int currentPromotionIndex { get; set; }

        void ResetDailyPrefers();

        //for appsflyer events for HUUUGE
        int videoFinishedCount { get; set; }
        int continousPlayCount { get; set; }
        int gameStartCount { get; set; }
        int gameFinishedCount { get; set; }
        DateTime appsFlyerLastLaunchTime { get; set; }
        int sessionCount { get; set; }
        int sessionsBeforePregameAdCount { get; set; }
        int pregameAdsPerDayCount { get; set; }
        DateTime intervalBetweenPregameAds { get; set; }
        bool autoPromotionToQueen { get; set; }

        Dictionary<string, Dictionary<string, int>> dailyResourceManager { get; set; }
        List<string> activePromotionSales { get; set; }
    }
}
