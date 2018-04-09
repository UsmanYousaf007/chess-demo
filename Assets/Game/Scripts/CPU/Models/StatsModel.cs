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
using UnityEngine;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public class StatsModel : IStatsModel
    {
        public Dictionary<int, PerformanceSet> stats { get; set; }

        [PostConstruct]
        public void LoadDefault()
        {
            Reset();
        }

        public void Reset()
        {
            stats = new Dictionary<int, PerformanceSet>();

            for (int i = 0; i < CPUSettings.DURATION_MINUTES.Length; i++)
            {
                // Create default performance set
                PerformanceSet pset;
                pset.performances = new List<Performance>();

                for (int j = 0; j < CPUSettings.MAX_STRENGTH; j++)
                {
                    // Create a default performance
                    pset.performances.Add(new Performance());
                }

                // Save the complete performance list for each duration
                stats.Add(i, pset);
            }
        }
    }
}
