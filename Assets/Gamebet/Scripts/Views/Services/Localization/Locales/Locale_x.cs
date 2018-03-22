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

namespace TurboLabz.Gamebet
{
    public class Locale_x : ILocale
    {
        public IDictionary<string, string> data { get; private set; }

        public Locale_x()
        {
            data = new Dictionary<string, string>() {
                // Client only keys

                #region Lobby

                { LocalizationKey.LOBBY_FREE_CURRENCY_1_BUTTON_LABEL, "x_Free Coins" },
                { LocalizationKey.LOBBY_PLAY_BUTTON_LABEL, "x_Play" },
                { LocalizationKey.LOBBY_FEEDBACK_BUTTON_LABEL, "x_Feedback" },

                #endregion

                #region PlayerProfile

                { LocalizationKey.PP_XP_MAX_LEVEL_LABEL, "x_MAX LEVEL" },
                { LocalizationKey.PP_CURRENCY_1_TITLE_LABEL, "x_Coins" },
                { LocalizationKey.PP_CURRENCY_2_TITLE_LABEL, "x_Bucks" },
                { LocalizationKey.PP_CURRENCY_1_WINNINGS_TITLE_LABEL, "x_Total Winnings" },
                { LocalizationKey.PP_GAMES_WON_TITLE_LABEL, "x_Games Won" },
                { LocalizationKey.PP_GAMES_LOST_TITLE_LABEL, "x_Games Lost" },
                { LocalizationKey.PP_GAMES_DRAWN_TITLE_LABEL, "x_Games Drawn" },
                { LocalizationKey.PP_WIN_RATE_TITLE_LABEL, "x_Win Percentage" },
                { LocalizationKey.PP_ROOM_TITLES_HEADER_LABEL, "x_Country Titles & Trophies" },
                { LocalizationKey.PP_NO_ROOM_TITLES_LABEL, "x_You don't have any titles yet." },

                #endregion

                // Client-server keys

                #region Rooms

                { "cuba1", "x_Cuba\n1 Min" },
                { "cuba2", "x_Cuba\n3 Mins" },
                { "cuba3", "x_Cuba\n5 Mins" },
                { "cuba4", "x_Cuba\n10 Mins" },
                { "england1", "x_England\n1 Min" },
                { "england2", "x_England\n3 Mins" },
                { "england3", "x_England\n5 Mins" },
                { "england4", "x_England\n10 Mins" },
                { "india1", "x_India\n1 Min" },
                { "india2", "x_India\n3 Mins" },
                { "india3", "x_India\n5 Mins" },
                { "india4", "x_India\n10 Mins" },
                { "germany1", "x_Germany\n1 Min" },
                { "germany2", "x_Germany\n3 Mins" },
                { "germany3", "x_Germany\n5 Mins" },
                { "germany4", "x_Germany\n10 Mins" },
                { "spain1", "x_Spain\n1 Min" },
                { "spain2", "x_Spain\n3 Mins" },
                { "spain3", "x_Spain\n5 Mins" },
                { "spain4", "x_Spain\n10 Mins" },
                { "france1", "x_France\n1 Min" },
                { "france2", "x_France\n3 Mins" },
                { "france3", "x_France\n5 Mins" },
                { "france4", "x_France\n10 Mins" },
                { "hungary1", "x_Hungary\n1 Min" },
                { "hungary2", "x_Hungary\n3 Mins" },
                { "hungary3", "x_Hungary\n5 Mins" },
                { "hungary4", "x_Hungary\n10 Mins" },
                { "ukraine1", "x_Ukraine\n1 Min" },
                { "ukraine2", "x_Ukraine\n3 Mins" },
                { "ukraine3", "x_Ukraine\n5 Mins" },
                { "ukraine4", "x_Ukraine\n10 Mins" },
                { "china1", "x_China\n1 Min" },
                { "china2", "x_China\n3 Mins" },
                { "china3", "x_China\n5 Mins" },
                { "china4", "x_China\n10 Mins" },
                { "usa1", "x_USA\n1 Min" },
                { "usa2", "x_USA\n3 Mins" },
                { "usa3", "x_USA\n5 Mins" },
                { "usa4", "x_USA\n10 Mins" },
                { "russia1", "x_Russia\n1 Min" },
                { "russia2", "x_Russia\n3 Mins" },
                { "russia3", "x_Russia\n5 Mins" },
                { "russia4", "x_Russia\n10 Mins" },

                #endregion

                { "none", "x_None" },

                #region Room titles

                { "clubChampion", "x_Club Champion" },
                { "proChampion", "x_Pro Champion" },
                { "nationalChampion", "x_National Champion" },

                #endregion

                #region Leagues

                { LocalizationKey.LEAGUE_1, "x_Iron League" },
                { LocalizationKey.LEAGUE_2, "x_Bronze League" },
                { LocalizationKey.LEAGUE_3, "x_Silver League" },
                { LocalizationKey.LEAGUE_4, "x_Gold League" },
                { LocalizationKey.LEAGUE_5, "x_Crystal League" },
                { LocalizationKey.LEAGUE_6, "x_Onyx League" },
                { LocalizationKey.LEAGUE_7, "x_Emerald League" },
                { LocalizationKey.LEAGUE_8, "x_Champion League" },
                { LocalizationKey.LEAGUE_9, "x_Royal League" },
                { LocalizationKey.LEAGUE_10, "x_Legend League" },

                #endregion
            };
        }
    }
}
