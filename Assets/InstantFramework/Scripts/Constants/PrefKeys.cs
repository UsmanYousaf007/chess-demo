/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-11 17:36:46 UTC+05:00
/// 
/// @description
/// [add_description_here]
using TurboLabz.Chess;

namespace TurboLabz.InstantGame
{
    public class PrefKeys
    {
        // FILENAME
        public const string PREFS_SAVE_FILENAME = "prefs";

        // PREFS
        public const string AUDIO_STATE = "audioState";
        public const string AD_SLOT_ID = "adSlotId";
        public const string AD_SLOT_IMPRESSIONS = "adSlotImpressions";
        public const string HAS_RATED = "hasRated";
        public const string IS_SAFE_MOVE_ON = "isSafeMoveOn";
        public const string IS_FRIEND_SCREEN_VISITED = "isFriendScreenVisited";
        public const string IS_COACH_TOOLTIP_SHOWN = "isCoachTooltipShown";
        public const string IS_STRENGTH_TOOLTIP_SHOWN = "isStrengthTooltipShown";
        public const string IS_LOBBY_LOADED_FIRST_TIME = "isLobbyLoadedFirstTime";
        public const string COACH_USED_COUNT = "coachUsedCount";
        public const string STRENGTH_USED_COUNT = "strengthUsedCount";
        public const string PROMOTION_CYCLE_INDEX = "promotionCycleIndex";
        public const string TIME_SPENT_1M_MATCH = "timeSpent1mMatch";
        public const string TIME_SPENT_3M_MATCH = "timeSpent3mMatch";
        public const string TIME_SPENT_5M_MATCH = "timeSpent5mMatch";
        public const string TIME_SPENT_10M_MATCH = "timeSpent10mMatch";
        public const string TIME_SPENT_30M_MATCH = "timeSpent30mMatch";
        public const string TIME_SPENT_LONG_MATCH = "timeSpentLongMatch";
        public const string TIME_SPENT_CPU_MATCH = "timeSpentCpuMatch";
        public const string TIME_SPENT_1M_TOURNAMENT = "timeSpent1mTournament";
        public const string TIME_SPENT_3M_TOURNAMENT = "timeSpent3mTournament";
        public const string TIME_SPENT_5M_TOURNAMENT = "timeSpent5mTournament";
        public const string TIME_SPENT_10M_TOURNAMENT = "timeSpent10mTournament";
        public const string LAST_LAUNCH_TIME = "lastLaunchDay";
        public const string SKIP_DLG_SHOWN = "skipDlgShown";
        public const string TIME_AT_SUBSCRIPTION_DLG_SHOWN = "timeAtSubscriptionDlgShown";
        public const string AUTO_SUBSCRIPTION_DLG_SHOWN_COUNT = "autoSubscriptionDlgShownCount";
        public const string RANKED_MATCHES_FINISHED_COUNT = "rankedMatchesFinishedCount";
        public const string AUTO_SUBSCRIPTION_DLG_SHOWN_FIRST_TIME = "autoSubscriptionDlgShownFirstTime";
        public const string FIRST_RANKED_GAME_OF_DAY = "firstRankedGameOfTheGame";
        public const string IS_INSTALL_DAY_OVER = "isInstallDayOver";
        public const string INSTALL_DAY_GAME_COUNT = "installDayGameCount";
        public const string INSTALL_DAY_FAV_MODE = "installDayFavMode";
        public const string OVERALL_FAV_MODE = "overallFavMode";
        public const string FAV_MODE_COUNT = "favModeCount";
        public const string GAME_COUNT_1M = "gameCount1m";
        public const string GAME_COUNT_3M = "gameCount3m";
        public const string GAME_COUNT_5M = "gameCount5m";
        public const string GAME_COUNT_10M = "gameCount10m";
        public const string GAME_COUNT_30M = "gameCount30m";
        public const string GAME_COUNT_LONG = "gameCountLong";
        public const string GAME_COUNT_CPU = "gameCountCPU";
        public const string LESSONS_COMPLETED = "lessonsCompleted";
        public const string CPU_POWERUPS_USED = "cpuPowerupsUsed";
        public const string INVENTORY_TAB_VISITED = "inventoryTabVisited";
        public const string SHOP_TAB_VISITED = "shopTabVisited";
        public const string THEMES_TAB_VISITED = "themesTabVisited";
        public const string CURRENT_PROMOTION_INDEX = "currentPromotionIndex";
        public const string ACTIVE_PROMOTION_SALES = "activePromotionSales";
        public const string IN_GAME_REMOVE_ADS_PROMOTION = "inGameRemoveAdsPromotionShown";
        public const string RATE_DLG_SHOWN_FIRST_TIME = "rateDialogueShownFirstTime";
        public const string FREE_DAILY_HINT = "freeDailyHint";
        public const string FREE_DAILY_RATING_BOOSTER = "freeDailyRatingBooster";
        public const string GAMES_PLAYED_PER_DAY = "gamesPlayedPerDay";

        public const string FREE_HINT = "freeHint";

        //Pregame ads
        public const string SESSIONS_BBEFORE_PREGAME_AD_COUNT = "sessionsBeforePregameAdCount";
        public const string PREGAME_ADS_PER_DAY_COUNT = "pregameAdsPerDayCount";
        public const string INTERVAL_BETWEEN_PREGAME_ADS = "intervalBetweenPregameAds";

        //for appsflyer events for HUUUGE
        public const string VIDEO_FINISHED_COUNT = "videoFinishedCount";
        public const string COUNTINOUS_PLAY_COUNT = "continousPlayCount";
        public const string GAME_START_COUNT = "gameStartCount";
        public const string GAME_FINISHED_COUNT = "gameFinishedCount";
        public const string APPS_FLYER_LAST_LAUNCH_TIME = "appsFlyerLastLaunchDay";
        public const string SESSION_COUNT = "sessionCount";

        public const string GLOBAL_ADS_COUNT = "globalAdsCount";
        public const string REWARDED_ADS_COUNT = "rewardedAdsCount";
        public const string INTERSTITIAL_ADS_COUNT = "interstitialAdsCount";
        public const string RESIGN_COUNT = "resignCount";
        public const string S3_URL_PING_VERSION = "urlPingVersion";
        public const string AUTO_PROMOTION_TO_QUEEN = "autoPromotionToQueen";

        public const string RESOURCE_USED = "resourceUsed";
        public const string RESOURCE_GEMS = "resourceGems";
        public const string RESOURCE_VIDEOS = "resourceVideos";
        public const string RESOURCE_FREE = "resourceFree";
        public const string RESOURCE_BUNDLE = "resourceBundle";

        public static readonly string[] DAILY_RESOURCE_MAMANGER = { RESOURCE_USED, RESOURCE_GEMS, RESOURCE_VIDEOS, RESOURCE_FREE, RESOURCE_BUNDLE };
    }
}
