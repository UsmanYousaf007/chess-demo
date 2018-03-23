/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-24 15:58:53 UTC+05:00
///
/// @description
/// [add_description_here]

using System.Collections.Generic;
using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;
using UnityEngine;
using TurboLabz.Chess;

namespace TurboLabz.InstantChess
{
    public class LoadCPUMenuCommand : Command
    {
        // Dispatch signals
        [Inject] public NavigatorEventSignal navigatorEventSignal { get; set; }
        [Inject] public UpdateCPUMenuViewSignal updateCPUMenuViewSignal { get; set; }
        [Inject] public LoadGameSignal loadGameSignal { get; set; }
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Models
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        // Service
        [Inject] public ILocalDataService localDataService { get; set; }

        public override void Execute()
        {
            chessboardModel.Reset();
            loadGameSignal.Dispatch();

            if (cpuGameModel.inProgress)
            {
                // Continue the game
                chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);
            }
            else
            {
                // Display the menu
                navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_MENU);
                updateCPUMenuViewSignal.Dispatch(cpuGameModel.GetCPUMenuVO());
            }
        }
    }
}
