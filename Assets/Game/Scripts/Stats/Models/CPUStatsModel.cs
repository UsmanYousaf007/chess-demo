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
using TurboLabz.InstantFramework;
using System;
using TurboLabz.CPU;

namespace TurboLabz.InstantGame
{
    public class CPUStatsModel : ICPUStatsModel
    {
        [Inject] public ILocalDataService localDataService { get; set; }

        public Dictionary<int, PerformanceSet> stats { get; set; }

        [PostConstruct]
        public void Load()
        {
            Reset();
         
            if (!localDataService.FileExists(SaveKeys.STATS_SAVE_FILENAME))
            {
                LogUtil.Log("No stats file found.", "cyan");
                return;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(SaveKeys.STATS_SAVE_FILENAME);
                Dictionary<int, string> statsSaveData = reader.ReadDictionary<int, string>(SaveKeys.STATS_DATA);

                foreach (KeyValuePair<int, string> entry in statsSaveData)
                {
                    stats[entry.Key] = JsonUtility.FromJson<PerformanceSet>(entry.Value);
                }

                reader.Close();
            }
            catch (Exception e)
            {
                LogUtil.Log("Corrupt saved stats! " + e, "red");
                localDataService.DeleteFile(SaveKeys.STATS_SAVE_FILENAME);
                Reset();
            }
        }

        public void Save(int durationIndex, int difficulty, int result)
        {
            try
            {
                // TODO: This is hacky where the strength index should be a proper independant index
                // and not calculated based off the strength value.
                int strengthIndex = difficulty - 1; 

                if (result <= stats[durationIndex].performance[strengthIndex])
                {
                    return;
                }

                stats[durationIndex].performance[strengthIndex] = result;

                ILocalDataWriter writer = localDataService.OpenWriter(SaveKeys.STATS_SAVE_FILENAME);

                Dictionary<int, string> statsSaveData = new Dictionary<int, string>();

                foreach (KeyValuePair<int, PerformanceSet> entry in stats)
                {
                    statsSaveData.Add(entry.Key, JsonUtility.ToJson(entry.Value));
                    LogUtil.Log("Added perf set:" + JsonUtility.ToJson(entry.Value), "cyan");
                }

                writer.WriteDictionary<int, string>(SaveKeys.STATS_DATA, statsSaveData);
                writer.Close();
            }
            catch (Exception e)
            {
                if (localDataService.FileExists(SaveKeys.STATS_SAVE_FILENAME))
                {
                    localDataService.DeleteFile(SaveKeys.STATS_SAVE_FILENAME);
                }

                LogUtil.Log("Critical error when saving stats. File deleted. " + e, "red");
            }
        }

        public void Reset()
        {
            stats = new Dictionary<int, PerformanceSet>();

            for (int i = 0; i < CPUSettings.DURATION_MINUTES.Length; i++)
            {
                // Create default performance set
                PerformanceSet pset = new PerformanceSet();
                pset.performance = new List<int>();

                for (int j = 0; j < CPUSettings.MAX_STRENGTH; j++)
                {
                    // Create a default performance
                    pset.performance.Add(StatResult.NONE);
                }

                // Save the complete performance list for each duration
                stats.Add(i, pset);
            }
        }
    }
}
