/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2020 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System;

namespace TurboLabz.InstantFramework
{
    public static class TournamentConstants
    {
        /// <summary>
        /// Buffer time to let players join the match if tournamentTimeLeft < findMatchTimeRequired
        /// </summary>
        public const int BUFFER_TIME_MINS = 2;

        public class ChestType
        {
            public const string EPIC = "epic";
            public const string RARE = "rare";
            public const string COMMON = "common";
        }

        public class TournamentType
        {
            public const string MIN_1 = "1Min";
            public const string MIN_5 = "5Min";
            public const string MIN_10 = "10Min";
        }

        public class LeagueType
        {
            public const string DIAMOND = "diamond";
            public const string SAPPHIRE = "sapphire";
            public const string EMERALD = "emerald";
            public const string PLATINUM = "platinum";
            public const string GOLD = "gold";
            public const string SILVER = "silver";
            public const string BRONZE = "bronze";
        }
    }
}
