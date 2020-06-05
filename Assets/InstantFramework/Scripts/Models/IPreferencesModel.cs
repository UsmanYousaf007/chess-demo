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
        float timeSpent1mMatch { get; set; }
        float timeSpent5mMatch { get; set; }
        float timeSpent10mMatch { get; set; }
        float timeSpentLongMatch { get; set; }
        float timeSpentCpuMatch { get; set; }
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
    }
}
