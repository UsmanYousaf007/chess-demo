﻿/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-04 13:31:38 UTC+05:00
/// 
/// @description
/// [add_description_here]
using TurboLabz.Chess;
using System.Collections.Generic;

namespace TurboLabz.InstantChess
{
    public interface IStatsModel
    {
        int durationIndex { get; set; }
        Dictionary<int, Performance> stats { get; set; }
        void Reset();
        StatsVO GetCPUStatsVO();
    }
}
