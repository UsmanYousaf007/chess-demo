/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-15 17:46:40 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;
using System;
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

namespace TurboLabz.CPU
{
    public class StartCPUGameCommand : Command
    {
        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }

        public override void Execute()
        {
            if (cpuGameModel.inProgress)
            {
                analyticsService.ComputerMatchContinued(cpuGameModel.levelId);

                chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);
            }
            else
            {
                analyticsService.ComputerMatchStarted(cpuGameModel.levelId);

                chessboardModel.gameDuration = TimeSpan.Zero;
                chessboardModel.aiMoveDelay = AiMoveDelay.CPU;
                    
                chessboardModel.playerColor = (UnityEngine.Random.Range(0,2) == 0) ? ChessColor.BLACK : ChessColor.WHITE;
                chessboardModel.opponentColor = (chessboardModel.playerColor == ChessColor.BLACK) ? ChessColor.WHITE : ChessColor.BLACK;

                chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);
            }
        }
    }
}
