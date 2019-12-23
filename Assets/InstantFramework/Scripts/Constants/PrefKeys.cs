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
        public const string TIME_AT_LOBBY_LOADED_FIRST_TIME = "timeAtLobbyLoadedFirstTime";

        //for appsflyer events for HUUUGE
        public const string VIDEO_FINISHED_COUNT = "videoFinishedCount";
        public const string COUNTINOUS_PLAY_COUNT = "continousPlayCount";
        public const string LAST_LAUNCH_TIME = "lastLaunchDay";
        public const string GAME_START_COUNT = "gameStartCount";
        public const string GAME_FINISHED_COUNT = "gameFinishedCount";
    }
}
