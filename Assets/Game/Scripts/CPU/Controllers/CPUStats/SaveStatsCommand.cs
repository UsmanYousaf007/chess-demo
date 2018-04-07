/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-11 11:42:52 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using System.Collections.Generic;
using TurboLabz.Chess;
using TurboLabz.TLUtils;
using System;
using UnityEngine;


namespace TurboLabz.InstantChess
{
    public class SaveStatsCommand : Command
    {
        // Parameters
        [Inject] public CPUStatsResultsVO result { get; set; }

        // Models
        [Inject] public IStatsModel statsModel { get; set; }

        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        public override void Execute()
        {
            try
            {
                ILocalDataWriter writer = localDataService.OpenWriter(SaveKeys.STATS_SAVE_FILENAME);

                Performance p = statsModel.stats[result.durationIndex].performances[result.strength];

                if (result.result == StatResult.WON)
                {
                    p.wins++;
                }
                else if (result.result == StatResult.LOST)
                {
                    p.losses++;
                }
                else if (result.result == StatResult.DRAWN)
                {
                    p.draws++;
                }

                statsModel.stats[result.durationIndex].performances[result.strength] = p;

                Dictionary<int, string> statsSaveData = new Dictionary<int, string>();

                foreach (KeyValuePair<int, PerformanceSet> entry in statsModel.stats)
                {
                    statsSaveData.Add(entry.Key, JsonUtility.ToJson(entry.Value));
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
    }
}
