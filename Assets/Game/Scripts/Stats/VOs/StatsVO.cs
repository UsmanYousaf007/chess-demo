/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2018 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2018-01-11 17:59:19 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;

namespace TurboLabz.InstantGame
{
    public struct StatsVO
    {
        public int[] durationMinutes;
        public Dictionary<int, PerformanceSet> stats;
        public double onlineWinPct;
        public int onlineWon;
        public int onlineLost;
        public int onlineDrawn;
        public int onlineTotal;
    }
}
