/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace TurboLabz.InstantFramework
{
    public interface IAdsSettingsModel
    {
        int slotHour { get; set; }
        int freeNoAdsPeriod { get; set; }
        int globalCap { get; set; }
        int rewardedVideoCap { get; set; }
        int interstitialCap { get; set; }
        int resignCap { get; set; }
        float minutesForVictoryInternalAd { get; set; }
        int autoSubscriptionDlgThreshold { get; set; }
        int daysPerAutoSubscriptionDlgThreshold { get; set; }
        int sessionsBeforePregameAd { get; set; }
        int maxPregameAdsPerDay { get; set; }
        int waitForPregameAdLoadSeconds { get; set; }
        int secondsBetweenIngameAds { get; set; }
        int secondsLeftDisableTournamentPregame { get; set; }
        int secondsElapsedDisable30MinInGame { get; set; }
        double intervalsBetweenPregameAds { get; set; }
        bool showPregameInOneMinute { get; set; }
        bool showPregameTournament { get; set; }
        bool showInGameCPU { get; set; }
        bool showInGame30Min { get; set; }
        bool showInGameClassic { get; set; }
        bool isBannerEnabled { get; set; }

        /*Timed ads settings*/
        int minPlayDaysRequired { get; set; }
        int minPurchasesRequired { get; set; }
        float premiumTimerCooldownTime { get; set; }
        float freemiumTimerCooldownTime { get; set; }

    }
}

