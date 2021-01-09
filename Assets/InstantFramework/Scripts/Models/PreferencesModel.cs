/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TurboLabz.InstantGame
{
    public class PreferencesModel : IPreferencesModel
    {
        // Services
        [Inject] public ILocalDataService localDataService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Listen to signals
        [Inject] public ModelsResetSignal modelsResetSignal { get; set; }
        [Inject] public ModelsLoadFromDiskSignal modelsLoadFromDiskSignal { get; set; }
        [Inject] public ModelsSaveToDiskSignal modelsSaveToDiskSignal { get; set; }

        public bool isAudioOn { get; set; }
        public long adSlotId { get; set; }
        public int adSlotImpressions { get; set; }
        public bool hasRated { get; set; }
        public bool isSafeMoveOn { get; set; }
        public bool isFriendScreenVisited { get; set; }
        public bool isCoachTooltipShown { get; set; }
        public bool isStrengthTooltipShown { get; set; }
        public bool isLobbyLoadedFirstTime { get; set; }
        public int coachUsedCount { get; set; }
        public int strengthUsedCount { get; set; }
        public int promotionCycleIndex { get; set; }
        public float timeSpent1mMatch { get; set; }
        public float timeSpent3mMatch { get; set; }
        public float timeSpent5mMatch { get; set; }
        public float timeSpent10mMatch { get; set; }
        public float timeSpent30mMatch { get; set; }
        public float timeSpentLongMatch { get; set; }
        public float timeSpentCpuMatch { get; set; }
        public DateTime lastLaunchTime { get; set; }
        public int videoFinishedCount { get; set; }
        public int continousPlayCount { get; set; }
        public int gameStartCount { get; set; }
        public int gameFinishedCount { get; set; }
        public DateTime appsFlyerLastLaunchTime { get; set; }
        public int globalAdsCount { get; set; }
        public int rewardedAdsCount { get; set; }
        public int interstitialAdsCount { get; set; }
        public int resignCount { get; set; }
        public bool isSkipVideoDlgShown { get; set; }
        public int sessionCount { get; set; }
        public DateTime timeAtSubscrptionDlgShown { get; set; }
        public int autoSubscriptionDlgShownCount { get; set; }
        public int sessionsBeforePregameAdCount { get; set; }
        public int pregameAdsPerDayCount { get; set; }
        public DateTime intervalBetweenPregameAds { get; set; }
        public bool isSubscriptionDlgShownOnFirstLaunch { get; set; }
        public bool autoPromotionToQueen { get; set;}
        public int rankedMatchesFinishedCount { get; set; }
        public bool isAutoSubsriptionDlgShownFirstTime { get; set; }
        public bool isFirstRankedGameOfTheDayFinished { get; set; }
        public bool isInstallDayOver { get; set; }
        public int installDayGameCount { get; set; }
        public string installDayFavMode { get; set; }
        public string overallFavMode { get; set; }
        public int favModeCount { get; set; }
        public int gameCount1m { get; set; }
        public int gameCount3m { get; set; }
        public int gameCount5m { get; set; }
        public int gameCount10m { get; set; }
        public int gameCount30m { get; set; }
        public int gameCountLong { get; set; }
        public int gameCountCPU { get; set; }
        public bool isAllLessonsCompleted { get; set; }
        public int cpuPowerUpsUsedCount { get; set; }
        public bool inventoryTabVisited { get; set; }
        public bool shopTabVisited { get; set; }
        public bool themesTabVisited { get; set; }
        public Dictionary<string, Dictionary<string, int>> dailyResourceManager { get; set; }
        public int currentPromotionIndex { get; set; }
        public List<string> activePromotionSales { get; set; }
        public bool inGameRemoveAdsPromotionShown { get; set; }
        public bool isRateAppDialogueFirstTimeShown { get; set; }
        public FreePowerUpStatus freeHint { get; set; }
        public FreePowerUpStatus freeDailyRatingBooster { get; set; }
        public int gamesPlayedPerDay { get; set; }

        [PostConstruct]
        public void PostConstruct()
        {
            modelsResetSignal.AddListener(Reset);
            modelsLoadFromDiskSignal.AddListener(LoadFromDisk);
            modelsSaveToDiskSignal.AddListener(SaveToDisk);
        }

        private void Reset()
        {
            isAudioOn = true;
            adSlotId = 0;
            adSlotImpressions = 0;
            hasRated = false;
            isSafeMoveOn = false;
            isFriendScreenVisited = false;
            isCoachTooltipShown = false;
            isStrengthTooltipShown = false;
            isLobbyLoadedFirstTime = false;
            coachUsedCount = 0;
            strengthUsedCount = 0;
            promotionCycleIndex = 0;
            lastLaunchTime = TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp);
            videoFinishedCount = 0;
            continousPlayCount = 0;
            gameStartCount = 0;
            gameFinishedCount = 0;
            appsFlyerLastLaunchTime = lastLaunchTime;
            isSkipVideoDlgShown = false;
            sessionCount = 0;
            timeAtSubscrptionDlgShown = DateTime.Now;
            autoSubscriptionDlgShownCount = 0;
            sessionsBeforePregameAdCount = 0;
            intervalBetweenPregameAds = DateTime.MaxValue;
            isSubscriptionDlgShownOnFirstLaunch = false;
            autoPromotionToQueen = false;
            rankedMatchesFinishedCount = 0;
            isAutoSubsriptionDlgShownFirstTime = false;
            isInstallDayOver = false;
            installDayFavMode = string.Empty;
            overallFavMode = string.Empty;
            installDayGameCount = 0;
            favModeCount = 0;
            gameCount1m = 0;
            gameCount3m = 0;
            gameCount5m = 0;
            gameCount10m = 0;
            gameCount30m = 0;
            gameCountLong = 0;
            gameCountCPU = 0;
            isAllLessonsCompleted = false;
            cpuPowerUpsUsedCount = 0;
            themesTabVisited = false;
            inventoryTabVisited = false;
            shopTabVisited = false;
            ResetDailyPrefers();
            isRateAppDialogueFirstTimeShown = false;
            freeHint = FreePowerUpStatus.NOT_CONSUMED;
            freeDailyRatingBooster = FreePowerUpStatus.NOT_CONSUMED;
        }

        private void LoadFromDisk()
        {
            if (!localDataService.FileExists(PrefKeys.PREFS_SAVE_FILENAME))
            {
                return;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(PrefKeys.PREFS_SAVE_FILENAME);

                // Read preferences here
                isAudioOn = reader.Read<bool>(PrefKeys.AUDIO_STATE);

                if (reader.HasKey(PrefKeys.AD_SLOT_ID))
                {
                    adSlotId = reader.Read<long>(PrefKeys.AD_SLOT_ID);
                    adSlotImpressions = reader.Read<int>(PrefKeys.AD_SLOT_IMPRESSIONS);
                }

                if (reader.HasKey(PrefKeys.HAS_RATED))
                {
                    hasRated = reader.Read<bool>(PrefKeys.HAS_RATED);
                }

                if (reader.HasKey(PrefKeys.IS_SAFE_MOVE_ON))
                {
                    isSafeMoveOn = false;
                }

                if (reader.HasKey(PrefKeys.IS_FRIEND_SCREEN_VISITED))
                {
                    isFriendScreenVisited = reader.Read<bool>(PrefKeys.IS_FRIEND_SCREEN_VISITED);
                }

                if (reader.HasKey(PrefKeys.IS_COACH_TOOLTIP_SHOWN))
                {
                    isCoachTooltipShown = reader.Read<bool>(PrefKeys.IS_COACH_TOOLTIP_SHOWN);
                }

                if (reader.HasKey(PrefKeys.IS_STRENGTH_TOOLTIP_SHOWN))
                {
                    isStrengthTooltipShown = reader.Read<bool>(PrefKeys.IS_STRENGTH_TOOLTIP_SHOWN);
                }

                if (reader.HasKey(PrefKeys.IS_LOBBY_LOADED_FIRST_TIME))
                {
                    isLobbyLoadedFirstTime = reader.Read<bool>(PrefKeys.IS_LOBBY_LOADED_FIRST_TIME);
                }

                if (reader.HasKey(PrefKeys.COACH_USED_COUNT))
                {
                    coachUsedCount = reader.Read<int>(PrefKeys.COACH_USED_COUNT);
                }

                if (reader.HasKey(PrefKeys.STRENGTH_USED_COUNT))
                {
                    strengthUsedCount = reader.Read<int>(PrefKeys.STRENGTH_USED_COUNT);
                }

                if (reader.HasKey(PrefKeys.PROMOTION_CYCLE_INDEX))
                {
                    promotionCycleIndex = reader.Read<int>(PrefKeys.PROMOTION_CYCLE_INDEX);
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_CPU_MATCH))
                {
                    timeSpentCpuMatch = reader.Read<float>(PrefKeys.TIME_SPENT_CPU_MATCH);
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_LONG_MATCH))
                {
                    timeSpentLongMatch = reader.Read<float>(PrefKeys.TIME_SPENT_LONG_MATCH);
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_1M_MATCH))
                {
                    timeSpent1mMatch = reader.Read<float>(PrefKeys.TIME_SPENT_1M_MATCH);
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_3M_MATCH))
                {
                    timeSpent1mMatch = reader.Read<float>(PrefKeys.TIME_SPENT_3M_MATCH);
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_5M_MATCH))
                {
                    timeSpent5mMatch = reader.Read<float>(PrefKeys.TIME_SPENT_5M_MATCH);
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_10M_MATCH))
                {
                    timeSpent10mMatch = reader.Read<float>(PrefKeys.TIME_SPENT_10M_MATCH);
                }

                if (reader.HasKey(PrefKeys.TIME_SPENT_30M_MATCH))
                {
                    timeSpent30mMatch = reader.Read<float>(PrefKeys.TIME_SPENT_30M_MATCH);
                }

                if (reader.HasKey(PrefKeys.LAST_LAUNCH_TIME))
                {
                    lastLaunchTime = DateTime.FromBinary(long.Parse(reader.Read<string>(PrefKeys.LAST_LAUNCH_TIME)));
                }

                if (reader.HasKey(PrefKeys.VIDEO_FINISHED_COUNT))
                {
                    videoFinishedCount = reader.Read<int>(PrefKeys.VIDEO_FINISHED_COUNT);
                }

                if (reader.HasKey(PrefKeys.COUNTINOUS_PLAY_COUNT))
                {
                    continousPlayCount = reader.Read<int>(PrefKeys.COUNTINOUS_PLAY_COUNT);
                }

                if (reader.HasKey(PrefKeys.GAME_START_COUNT))
                {
                    gameStartCount = reader.Read<int>(PrefKeys.GAME_START_COUNT);
                }

                if (reader.HasKey(PrefKeys.GAME_FINISHED_COUNT))
                {
                    gameFinishedCount = reader.Read<int>(PrefKeys.GAME_FINISHED_COUNT);
                }

                if (reader.HasKey(PrefKeys.APPS_FLYER_LAST_LAUNCH_TIME))
                {
                    appsFlyerLastLaunchTime = DateTime.FromBinary(long.Parse(reader.Read<string>(PrefKeys.APPS_FLYER_LAST_LAUNCH_TIME)));
                }

                if (reader.HasKey(PrefKeys.GLOBAL_ADS_COUNT))
                {
                    globalAdsCount = reader.Read<int>(PrefKeys.GLOBAL_ADS_COUNT);
                }

                if (reader.HasKey(PrefKeys.REWARDED_ADS_COUNT))
                {
                    rewardedAdsCount = reader.Read<int>(PrefKeys.REWARDED_ADS_COUNT);
                }

                if (reader.HasKey(PrefKeys.INTERSTITIAL_ADS_COUNT))
                {
                    interstitialAdsCount = reader.Read<int>(PrefKeys.INTERSTITIAL_ADS_COUNT);
                }

                if (reader.HasKey(PrefKeys.RESIGN_COUNT))
                {
                    resignCount = reader.Read<int>(PrefKeys.RESIGN_COUNT);
                }

                if (reader.HasKey(PrefKeys.SKIP_DLG_SHOWN))
                {
                    isSkipVideoDlgShown = reader.Read<bool>(PrefKeys.SKIP_DLG_SHOWN);
                }

                if (reader.HasKey(PrefKeys.SESSION_COUNT))
                {
                    sessionCount = reader.Read<int>(PrefKeys.SESSION_COUNT);
                }

                if (reader.HasKey(PrefKeys.TIME_AT_SUBSCRIPTION_DLG_SHOWN))
                {
                    timeAtSubscrptionDlgShown = DateTime.FromBinary(long.Parse(reader.Read<string>(PrefKeys.TIME_AT_SUBSCRIPTION_DLG_SHOWN)));
                }

                if (reader.HasKey(PrefKeys.AUTO_SUBSCRIPTION_DLG_SHOWN_COUNT))
                {
                    autoSubscriptionDlgShownCount = reader.Read<int>(PrefKeys.AUTO_SUBSCRIPTION_DLG_SHOWN_COUNT);
                }

                if (reader.HasKey(PrefKeys.SESSIONS_BBEFORE_PREGAME_AD_COUNT))
                {
                    sessionsBeforePregameAdCount = reader.Read<int>(PrefKeys.SESSIONS_BBEFORE_PREGAME_AD_COUNT);
                }

                if (reader.HasKey(PrefKeys.PREGAME_ADS_PER_DAY_COUNT))
                {
                    pregameAdsPerDayCount = reader.Read<int>(PrefKeys.PREGAME_ADS_PER_DAY_COUNT);
                }

                if (reader.HasKey(PrefKeys.INTERVAL_BETWEEN_PREGAME_ADS))
                {
                    intervalBetweenPregameAds = DateTime.FromBinary(long.Parse(reader.Read<string>(PrefKeys.INTERVAL_BETWEEN_PREGAME_ADS)));
                }

                if (reader.HasKey(PrefKeys.AUTO_PROMOTION_TO_QUEEN))
                {
                    autoPromotionToQueen = reader.Read<bool>(PrefKeys.AUTO_PROMOTION_TO_QUEEN);
                }

                if (reader.HasKey(PrefKeys.RANKED_MATCHES_FINISHED_COUNT))
                {
                    rankedMatchesFinishedCount = reader.Read<int>(PrefKeys.RANKED_MATCHES_FINISHED_COUNT);
                }

                if (reader.HasKey(PrefKeys.AUTO_SUBSCRIPTION_DLG_SHOWN_FIRST_TIME))
                {
                    isAutoSubsriptionDlgShownFirstTime = reader.Read<bool>(PrefKeys.AUTO_SUBSCRIPTION_DLG_SHOWN_FIRST_TIME);
                }

                if (reader.HasKey(PrefKeys.FIRST_RANKED_GAME_OF_DAY))
                {
                    isFirstRankedGameOfTheDayFinished = reader.Read<bool>(PrefKeys.FIRST_RANKED_GAME_OF_DAY);
                }

                if (reader.HasKey(PrefKeys.IS_INSTALL_DAY_OVER))
                {
                    isInstallDayOver = reader.Read<bool>(PrefKeys.IS_INSTALL_DAY_OVER);
                }

                if (reader.HasKey(PrefKeys.INSTALL_DAY_GAME_COUNT))
                {
                    installDayGameCount = reader.Read<int>(PrefKeys.INSTALL_DAY_GAME_COUNT);
                }

                if (reader.HasKey(PrefKeys.INSTALL_DAY_FAV_MODE))
                {
                    installDayFavMode = reader.Read<string>(PrefKeys.INSTALL_DAY_FAV_MODE);
                }

                if (reader.HasKey(PrefKeys.OVERALL_FAV_MODE))
                {
                    overallFavMode = reader.Read<string>(PrefKeys.OVERALL_FAV_MODE);
                }

                if (reader.HasKey(PrefKeys.FAV_MODE_COUNT))
                {
                    favModeCount = reader.Read<int>(PrefKeys.FAV_MODE_COUNT);
                }

                if (reader.HasKey(PrefKeys.GAME_COUNT_1M))
                {
                    gameCount1m = reader.Read<int>(PrefKeys.GAME_COUNT_1M);
                }

                if (reader.HasKey(PrefKeys.GAME_COUNT_3M))
                {
                    gameCount1m = reader.Read<int>(PrefKeys.GAME_COUNT_3M);
                }

                if (reader.HasKey(PrefKeys.GAME_COUNT_5M))
                {
                    gameCount5m = reader.Read<int>(PrefKeys.GAME_COUNT_5M);
                }

                if (reader.HasKey(PrefKeys.GAME_COUNT_10M))
                {
                    gameCount10m = reader.Read<int>(PrefKeys.GAME_COUNT_10M);
                }

                if (reader.HasKey(PrefKeys.GAME_COUNT_30M))
                {
                    gameCount30m = reader.Read<int>(PrefKeys.GAME_COUNT_30M);
                }

                if (reader.HasKey(PrefKeys.GAME_COUNT_LONG))
                {
                    gameCountLong = reader.Read<int>(PrefKeys.GAME_COUNT_LONG);
                }

                if (reader.HasKey(PrefKeys.GAME_COUNT_CPU))
                {
                    gameCountCPU = reader.Read<int>(PrefKeys.GAME_COUNT_CPU);
                }

                if (reader.HasKey(PrefKeys.LESSONS_COMPLETED))
                {
                    isAllLessonsCompleted = reader.Read<bool>(PrefKeys.LESSONS_COMPLETED);
                }

                if (reader.HasKey(PrefKeys.CPU_POWERUPS_USED))
                {
                    cpuPowerUpsUsedCount = reader.Read<int>(PrefKeys.CPU_POWERUPS_USED);
                }

                if (reader.HasKey(PrefKeys.INVENTORY_TAB_VISITED))
                {
                    inventoryTabVisited = reader.Read<bool>(PrefKeys.INVENTORY_TAB_VISITED);
                }

                if (reader.HasKey(PrefKeys.SHOP_TAB_VISITED))
                {
                    shopTabVisited = reader.Read<bool>(PrefKeys.SHOP_TAB_VISITED);
                }

                if (reader.HasKey(PrefKeys.THEMES_TAB_VISITED))
                {
                    themesTabVisited = reader.Read<bool>(PrefKeys.THEMES_TAB_VISITED);
                }

                if (reader.HasKey(PrefKeys.CURRENT_PROMOTION_INDEX))
                {
                    currentPromotionIndex = reader.Read<int>(PrefKeys.CURRENT_PROMOTION_INDEX);
                }

                if (reader.HasKey(PrefKeys.IN_GAME_REMOVE_ADS_PROMOTION))
                {
                    inGameRemoveAdsPromotionShown = reader.Read<bool>(PrefKeys.IN_GAME_REMOVE_ADS_PROMOTION);
                }

                if (reader.HasKey(PrefKeys.RATE_DLG_SHOWN_FIRST_TIME))
                {
                    isRateAppDialogueFirstTimeShown = reader.Read<bool>(PrefKeys.RATE_DLG_SHOWN_FIRST_TIME);
                }

                if (reader.HasKey(PrefKeys.FREE_HINT))
                {
                    freeHint = reader.Read<FreePowerUpStatus>(PrefKeys.FREE_HINT);
                }

                if (reader.HasKey(PrefKeys.FREE_DAILY_RATING_BOOSTER))
                {
                    freeDailyRatingBooster = reader.Read<FreePowerUpStatus>(PrefKeys.FREE_DAILY_RATING_BOOSTER);
                }

                if (reader.HasKey(PrefKeys.GAMES_PLAYED_PER_DAY))
                {
                    gamesPlayedPerDay = reader.Read<int>(PrefKeys.GAMES_PLAYED_PER_DAY);
                }



                var transactionKeys = dailyResourceManager.Keys.ToList();

                foreach (var transaction in transactionKeys)
                {
                    var resourceKeys = dailyResourceManager[transaction].Keys.ToList();

                    foreach (var resource in resourceKeys)
                    {
                        var key = $"{transaction}{resource}";

                        if (reader.HasKey(key))
                        {
                            dailyResourceManager[transaction][resource] = reader.Read<int>(key);
                        }
                    }
                }

                int i = 0;
                while (reader.HasKey($"{PrefKeys.ACTIVE_PROMOTION_SALES}{i}"))
                {
                    activePromotionSales.Add(reader.Read<string>($"{PrefKeys.ACTIVE_PROMOTION_SALES}{i}"));
                    i++;
                }

                reader.Close();
            }
            catch (Exception e)
            {
                LogUtil.Log("Corrupt saved prefs! " + e, "red");
                localDataService.DeleteFile(PrefKeys.PREFS_SAVE_FILENAME);
                Reset();
            }
        }

        private void SaveToDisk()
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(PrefKeys.PREFS_SAVE_FILENAME);

                // Save preferences here
                writer.Write<bool>(PrefKeys.AUDIO_STATE, isAudioOn);
                writer.Write<long>(PrefKeys.AD_SLOT_ID, adSlotId);
                writer.Write<int>(PrefKeys.AD_SLOT_IMPRESSIONS, adSlotImpressions);
                writer.Write<bool>(PrefKeys.HAS_RATED, hasRated);
                writer.Write<bool>(PrefKeys.IS_SAFE_MOVE_ON, isSafeMoveOn);
                writer.Write<bool>(PrefKeys.IS_FRIEND_SCREEN_VISITED, isFriendScreenVisited);
                writer.Write<bool>(PrefKeys.IS_COACH_TOOLTIP_SHOWN, isCoachTooltipShown);
                writer.Write<bool>(PrefKeys.IS_STRENGTH_TOOLTIP_SHOWN, isStrengthTooltipShown);
                writer.Write<bool>(PrefKeys.IS_LOBBY_LOADED_FIRST_TIME, isLobbyLoadedFirstTime);
                writer.Write<int>(PrefKeys.COACH_USED_COUNT, coachUsedCount);
                writer.Write<int>(PrefKeys.STRENGTH_USED_COUNT, strengthUsedCount);
                writer.Write<int>(PrefKeys.PROMOTION_CYCLE_INDEX, promotionCycleIndex);
                writer.Write<float>(PrefKeys.TIME_SPENT_1M_MATCH, timeSpent1mMatch);
                writer.Write<float>(PrefKeys.TIME_SPENT_3M_MATCH, timeSpent3mMatch);
                writer.Write<float>(PrefKeys.TIME_SPENT_5M_MATCH, timeSpent5mMatch);
                writer.Write<float>(PrefKeys.TIME_SPENT_10M_MATCH, timeSpent10mMatch);
                writer.Write<float>(PrefKeys.TIME_SPENT_30M_MATCH, timeSpent30mMatch);
                writer.Write<float>(PrefKeys.TIME_SPENT_LONG_MATCH, timeSpentLongMatch);
                writer.Write<float>(PrefKeys.TIME_SPENT_CPU_MATCH, timeSpentCpuMatch);
                writer.Write<string>(PrefKeys.LAST_LAUNCH_TIME, lastLaunchTime.ToBinary().ToString());
                writer.Write<int>(PrefKeys.VIDEO_FINISHED_COUNT, videoFinishedCount);
                writer.Write<int>(PrefKeys.COUNTINOUS_PLAY_COUNT, continousPlayCount);
                writer.Write<int>(PrefKeys.GAME_START_COUNT, gameStartCount);
                writer.Write<int>(PrefKeys.GAME_FINISHED_COUNT, gameFinishedCount);
                writer.Write<string>(PrefKeys.APPS_FLYER_LAST_LAUNCH_TIME, appsFlyerLastLaunchTime.ToBinary().ToString());
                writer.Write<int>(PrefKeys.GLOBAL_ADS_COUNT, globalAdsCount);
                writer.Write<int>(PrefKeys.REWARDED_ADS_COUNT, globalAdsCount);
                writer.Write<int>(PrefKeys.INTERSTITIAL_ADS_COUNT, interstitialAdsCount);
                writer.Write<int>(PrefKeys.RESIGN_COUNT, resignCount);
                writer.Write<bool>(PrefKeys.SKIP_DLG_SHOWN, isSkipVideoDlgShown);
                writer.Write<int>(PrefKeys.SESSION_COUNT, sessionCount);
                writer.Write<string>(PrefKeys.TIME_AT_SUBSCRIPTION_DLG_SHOWN, timeAtSubscrptionDlgShown.ToBinary().ToString());
                writer.Write<int>(PrefKeys.AUTO_SUBSCRIPTION_DLG_SHOWN_COUNT, autoSubscriptionDlgShownCount);
                writer.Write<int>(PrefKeys.SESSIONS_BBEFORE_PREGAME_AD_COUNT, sessionsBeforePregameAdCount);
                writer.Write<int>(PrefKeys.PREGAME_ADS_PER_DAY_COUNT, pregameAdsPerDayCount);
                writer.Write<string>(PrefKeys.INTERVAL_BETWEEN_PREGAME_ADS, intervalBetweenPregameAds.ToBinary().ToString());
                writer.Write<bool>(PrefKeys.AUTO_PROMOTION_TO_QUEEN, autoPromotionToQueen);
                writer.Write<int>(PrefKeys.RANKED_MATCHES_FINISHED_COUNT, rankedMatchesFinishedCount);
                writer.Write<bool>(PrefKeys.AUTO_SUBSCRIPTION_DLG_SHOWN_FIRST_TIME, isAutoSubsriptionDlgShownFirstTime);
                writer.Write<bool>(PrefKeys.FIRST_RANKED_GAME_OF_DAY, isFirstRankedGameOfTheDayFinished);
                writer.Write<bool>(PrefKeys.IS_INSTALL_DAY_OVER, isInstallDayOver);
                writer.Write<int>(PrefKeys.INSTALL_DAY_GAME_COUNT, installDayGameCount);
                writer.Write<string>(PrefKeys.INSTALL_DAY_FAV_MODE, installDayFavMode);
                writer.Write<string>(PrefKeys.OVERALL_FAV_MODE, overallFavMode);
                writer.Write<int>(PrefKeys.FAV_MODE_COUNT, favModeCount);
                writer.Write<int>(PrefKeys.GAME_COUNT_1M, gameCount1m);
                writer.Write<int>(PrefKeys.GAME_COUNT_3M, gameCount3m);
                writer.Write<int>(PrefKeys.GAME_COUNT_5M, gameCount5m);
                writer.Write<int>(PrefKeys.GAME_COUNT_10M, gameCount10m);
                writer.Write<int>(PrefKeys.GAME_COUNT_30M, gameCount30m);
                writer.Write<int>(PrefKeys.GAME_COUNT_LONG, gameCountLong);
                writer.Write<int>(PrefKeys.GAME_COUNT_CPU, gameCountCPU);
                writer.Write<bool>(PrefKeys.LESSONS_COMPLETED, isAllLessonsCompleted);
                writer.Write<int>(PrefKeys.CPU_POWERUPS_USED, cpuPowerUpsUsedCount);
                writer.Write<bool>(PrefKeys.INVENTORY_TAB_VISITED, inventoryTabVisited);
                writer.Write<bool>(PrefKeys.SHOP_TAB_VISITED, shopTabVisited);
                writer.Write<bool>(PrefKeys.THEMES_TAB_VISITED, themesTabVisited);
                writer.Write<int>(PrefKeys.CURRENT_PROMOTION_INDEX, currentPromotionIndex);
                writer.Write<bool>(PrefKeys.IN_GAME_REMOVE_ADS_PROMOTION, inGameRemoveAdsPromotionShown);
                writer.Write<bool>(PrefKeys.RATE_DLG_SHOWN_FIRST_TIME, isRateAppDialogueFirstTimeShown);
                writer.Write<FreePowerUpStatus>(PrefKeys.FREE_HINT, freeHint);
                writer.Write<FreePowerUpStatus>(PrefKeys.FREE_DAILY_RATING_BOOSTER, freeDailyRatingBooster);
                writer.Write<int>(PrefKeys.GAMES_PLAYED_PER_DAY, gamesPlayedPerDay);



                foreach (var transaction in dailyResourceManager)
                {
                    foreach (var resource in transaction.Value)
                    {
                        writer.Write<int>($"{transaction.Key}{resource.Key}", resource.Value);
                    }
                }

                int i = 0;
                foreach (var sale in activePromotionSales)
                {
                    writer.Write<string>($"{PrefKeys.ACTIVE_PROMOTION_SALES}{i}", sale);
                    i++;
                }

                writer.Close();
            }
            catch (Exception e)
            {
                if (localDataService.FileExists(PrefKeys.PREFS_SAVE_FILENAME))
                {
                    localDataService.DeleteFile(PrefKeys.PREFS_SAVE_FILENAME);
                }

                LogUtil.Log("Critical error when saving prefs. File deleted. " + e, "red");
            }
        }

        public void ResetDailyPrefers()
        {
            lastLaunchTime = TimeUtil.ToDateTime(backendService.serverClock.currentTimestamp).ToLocalTime();
            timeSpentCpuMatch = 0;
            timeSpentLongMatch = 0;
            timeSpent1mMatch = 0;
            timeSpent5mMatch = 0;
            timeSpent10mMatch = 0;
            timeSpent30mMatch = 0;
            globalAdsCount = 0;
            rewardedAdsCount = 0;
            interstitialAdsCount = 0;
            resignCount = 0;
            pregameAdsPerDayCount = 0;
            isFirstRankedGameOfTheDayFinished = false;
            currentPromotionIndex = 0;
            inGameRemoveAdsPromotionShown = false;
            gamesPlayedPerDay = 0;
            dailyResourceManager = new Dictionary<string, Dictionary<string, int>>();
            activePromotionSales = new List<string>();

            //if (freeHint != FreePowerUpStatus.BOUGHT)
            //    freeHint = FreePowerUpStatus.NOT_CONSUMED;

            if (freeDailyRatingBooster != FreePowerUpStatus.BOUGHT)
                freeDailyRatingBooster = FreePowerUpStatus.NOT_CONSUMED;

            foreach (var key in PrefKeys.DAILY_RESOURCE_MAMANGER)
            {
                var value = new Dictionary<string, int>();
                value.Add(GSBackendKeys.ShopItem.SPECIAL_ITEM_TICKET, 0);
                value.Add(GSBackendKeys.ShopItem.SPECIAL_ITEM_RATING_BOOSTER, 0);
                value.Add(GSBackendKeys.ShopItem.SPECIAL_ITEM_HINT, 0);
                value.Add(GSBackendKeys.ShopItem.SPECIAL_ITEM_KEY, 0);
                dailyResourceManager.Add(key, value);
            }
        }
    }
}
