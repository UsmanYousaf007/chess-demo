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
using TurboLabz.CPU;

namespace TurboLabz.InstantGame
{
    public class LoadStatsCommand : Command
    {
        // Dispatch Signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateStatsSignal updateStatsSignal { get; set; }

        // Models
        [Inject] public ICPUStatsModel cpuStatsModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public ILocalDataService localDataService { get; set; }

        public override void Execute()
        {
            navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_STATS);

            StatsVO vo = new StatsVO();
            vo.durationMinutes = CPUSettings.DURATION_MINUTES;
            vo.stats = cpuStatsModel.stats;

            int won = playerModel.totalGamesWon;
            int lost = playerModel.totalGamesLost;
            int drawn = playerModel.totalGamesDrawn;

            int totalGames = won + lost + drawn;
            double winPct = (totalGames > 0) ? Math.Round((double)won / (double)totalGames, 2) * 100 : 0;

            vo.onlineWinPct = winPct;
            vo.onlineWon = won;
            vo.onlineLost = lost;
            vo.onlineDrawn = drawn;
            vo.onlineTotal = totalGames;

            updateStatsSignal.Dispatch(vo);
        }
    }
}
