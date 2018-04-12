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
        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateStatsSignal updateStatsSignal { get; set; }

        // Models
        [Inject] public IStatsModel statsModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_STATS);

            CPUStatsVO vo = new CPUStatsVO();
            vo.maxStrength = CPUSettings.MAX_STRENGTH;
            vo.durationMinutes = CPUSettings.DURATION_MINUTES;
            vo.selectedStrengthIndex = cpuGameModel.cpuStrength - 1;
            vo.stats = statsModel.stats;

            updateStatsSignal.Dispatch(vo);
        }
    }
}
