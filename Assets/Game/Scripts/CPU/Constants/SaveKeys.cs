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
    public class SaveKeys
    {
        // FILENAME
        public const string CPU_SAVE_FILENAME = "cpuSave1";
        public const string STATS_SAVE_FILENAME = "statsSave1";

        // CPU MENU MODEL
        public const string CPU_STRENGTH = "cpuStrength";
        public const string DURATION_INDEX = "durationIndex";
        public const string IN_PROGRESS = "inProgress";
        public const string PLAYER_COLOR_INDEX = "playerColorIndex";
        public const string DEV_FEN = "devFen";
        public const string TOTAL_GAMES = "totalGames";

        // CHESSBOARD MODEL
        public const string GAME_DURATION = "gameDuration";
        public const string PLAYER_TIMER = "playerTimer";
        public const string OPPONENT_TIMER = "opponentTimer";
        public const string PLAYER_COLOR = "playerColor";
        public const string OPPONENT_COLOR = "opponentColor";
        public const string MOVE_LIST = "moveList";
        public const string TRIMMED_MOVE_LIST = "trimmedMoveList";
        public const string USED_HELP = "usedHelp";
        public const string POWER_MODE = "powerMode";
        public const string FREE_HINTS = "freeHints";

        // STATS MODEL
        public const string STATS_DATA = "statsData";
    }
}
