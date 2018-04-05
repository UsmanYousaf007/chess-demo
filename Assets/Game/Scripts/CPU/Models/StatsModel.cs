/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-12-14 13:37:03 UTC+05:00
/// 
/// @description
/// [add_description_here]
using System.Collections.Generic;

namespace TurboLabz.InstantChess
{
    public class StatsModel : IStatsModel
    {
        public int durationIndex { get; set; }
        public Dictionary<int, Performance> stats { get; set; }

        public StatsVO GetCPUStatsVO()
        {
            StatsVO vo = new StatsVO();
            vo.durationIndex = durationIndex;
            vo.stats = stats;
            vo.durationMinutes = CPUSettings.DURATION_MINUTES;

            return vo;
        }

        public void Reset()
        {
            durationIndex = 0;
            stats = new Dictionary<int, Performance>();
        }
    }
}
