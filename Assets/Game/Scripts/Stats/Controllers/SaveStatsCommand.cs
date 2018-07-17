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
using TurboLabz.CPU;


namespace TurboLabz.InstantGame
{
    public class SaveStatsCommand : Command
    {
        // Parameters
        [Inject] public int result { get; set; }

        // Models
        [Inject] public ICPUStatsModel statsModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        public override void Execute()
        {
            int cpuStrength = cpuGameModel.cpuStrength;
            statsModel.Save(cpuStrength, result);

            LogUtil.Log("SAVING STATS...", "cyan");
        }
    }
}
