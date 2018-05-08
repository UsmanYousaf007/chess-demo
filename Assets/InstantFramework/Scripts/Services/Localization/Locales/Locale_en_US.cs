/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-05 16:43:41 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;

namespace TurboLabz.InstantFramework
{
    public class Locale_en_US : ILocale
    {
        public IDictionary<string, string> data { get; private set; }

        public Locale_en_US()
        {
            data = new Dictionary<string, string>() {
                //
                // Client only values
                //

                #region Game

                { LocalizationKey.GM_RESULT_DIALOG_HEADING_WIN, "VICTORY" },
                { LocalizationKey.GM_RESULT_DIALOG_HEADING_LOSE, "DEFEAT" },
                { LocalizationKey.GM_RESULT_DIALOG_HEADING_DRAW, "DRAW" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_CHECKMATE, "By Checkmate" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_STALEMATE, "By Stalemate" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_INSUFFICIENT_MATERIAL, "By Insufficient Material" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_FIFTY_MOVE_RULE, "By Fifty Move Rule" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_DRAW_BY_THREEFOLD_REPEAT_RULE, "By Threefold Repeat Rule" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_TIMER_EXPIRED, "By Time" },
                { LocalizationKey.GM_RESULT_DIALOG_REASON_RESIGNATION, "By Resignation" },
                { LocalizationKey.GM_DRAW_DIALOG_CLAIM_BY_FIFTY_MOVE_RULE, "Claim draw by Fifty Move rule?" },
                { LocalizationKey.GM_DRAW_DIALOG_CLAIM_BY_THREEFOLD_REPEAT_RULE, "Claim draw by Threefold Repeat rule?" },
                { LocalizationKey.GM_DRAW_DIALOG_YES_BUTTON, "YES" },
                { LocalizationKey.GM_DRAW_DIALOG_NO_BUTTON, "NO" },
                { LocalizationKey.GM_ROOM_DURATION, "{0} m" },

                #endregion

                //
                // Client-server key-values
                //

                #region CPUMenu

                { LocalizationKey.CPU_MENU_STRENGTH, "Difficulty" },
                { LocalizationKey.CPU_MENU_DURATION, "Time Limit" },
                { LocalizationKey.CPU_MENU_DURATION_NONE, "None" },
                { LocalizationKey.CPU_MENU_PLAYER_COLOR, "Play As" },
                { LocalizationKey.CPU_MENU_PLAY, "PLAY" },
                { LocalizationKey.CPU_MENU_STATS, "STATS" },
                { LocalizationKey.CPU_GAME_CPU_NAME, "Computer" },
                { LocalizationKey.CPU_GAME_CPU_STRENGTH, "Difficulty {0}" },
                { LocalizationKey.CPU_GAME_PLAYER_NAME, "Player" },

                { LocalizationKey.CPU_GAME_RESIGN_BUTTON, "RESIGN GAME" },
                { LocalizationKey.CPU_GAME_UNDO_BUTTON, "UNDO" },
                { LocalizationKey.CPU_GAME_HINT_BUTTON, "GET HINT" },
                { LocalizationKey.CPU_GAME_EXIT_DLG_TITLE, "Menu" },
                { LocalizationKey.CPU_GAME_CONTINUE_BUTTON, "RESUME" },
                { LocalizationKey.CPU_RESULTS_STATS_BUTTON, "VIEW STATS" },
                { LocalizationKey.CPU_RESULTS_EXIT_BUTTON, "BACK TO MENU" },

                #endregion

                #region Stats

                { LocalizationKey.STATS_TITLE, "Stats" },
                { LocalizationKey.STATS_DIFFICULTY, "Difficulty Level" },
                { LocalizationKey.STATS_TOTAL_GAMES, "Total Games" },
                { LocalizationKey.STATS_WON, "Won" },
                { LocalizationKey.STATS_WON_WITH_HELP, "Won" },
                { LocalizationKey.STATS_WON_WITH_HELP_EXPLAINER, "With Undos | Hints" },
                { LocalizationKey.STATS_DURATION, "Duration" },
                { LocalizationKey.STATS_DURATION_TIME, "{0} m" },

                #endregion

                #region Share

                { LocalizationKey.SHARE_STANDARD, "Hey, let's play Instant Chess!" },

                #endregion
            };
        }
    }
}
