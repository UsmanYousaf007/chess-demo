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
using System;
using TurboLabz.TLUtils;
using UnityEngine;


namespace TurboLabz.InstantChess
{
    public class LoadStatsCommand : Command
    {
        // Parameters
        [Inject] public int durationIndex { get; set; }

        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateStatsSignal updateStatsSignal { get; set; }

        // Models
        [Inject] public IStatsModel statsModel { get; set; }

        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        public override void Execute()
        {
            statsModel.Reset();

            statsModel.durationIndex = durationIndex;

            if (!localDataService.FileExists(SaveKeys.STATS_SAVE_FILENAME))
            {
                LogUtil.Log("No saved stats found.", "yellow");
                LoadStats();
                return;
            }

            try
            {
                ILocalDataReader reader = localDataService.OpenReader(SaveKeys.STATS_SAVE_FILENAME);

                // STATS MODEL
                Dictionary<int, string> statsSaveData = new Dictionary<int, string>();
                statsSaveData = reader.ReadDictionary<int, string>(SaveKeys.STATS_DATA);

                foreach (KeyValuePair<int, string> entry in statsSaveData)
                {
                    Performance p = JsonUtility.FromJson<Performance>(entry.Value);
                    statsModel.stats[entry.Key] = p;
                }

                reader.Close();
            }
            catch (Exception e)
            {
                LogUtil.Log("Corrupt saved stats! " + e, "red");
                localDataService.DeleteFile(SaveKeys.STATS_SAVE_FILENAME);
                statsModel.Reset();
                LoadStats();
            }
        }

        private void LoadStats()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_STATS);
        }
    }
}
