/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2016 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2016-10-15 17:46:40 UTC+05:00
/// 
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;
using System;
using TurboLabz.TLUtils;

namespace TurboLabz.InstantChess
{
    public class StartNewCPUGameCommand : Command
    {
        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        public override void Execute()
        {
            chessboardModel.Reset();

            if (cpuGameModel.durationIndex == 0)
            {
                chessboardModel.gameDuration = TimeSpan.Zero;
            }
            else
            {
                long gameDuration = CPUSettings.DURATION_MINUTES[cpuGameModel.durationIndex] * 60000;
                chessboardModel.gameDuration = TimeSpan.FromMilliseconds(gameDuration);

                chessboardModel.aiTimeControl = AiTimeControl.CPU;
                chessboardModel.playerTimer = chessboardModel.gameDuration;
                chessboardModel.opponentTimer = chessboardModel.gameDuration;
            }
                
            ChessColor playerPreferredColor = CPUSettings.PLAYER_COLORS[cpuGameModel.playerColorIndex];

            if (playerPreferredColor == ChessColor.NONE)
            {
                playerPreferredColor = (UnityEngine.Random.Range(0,2) == 0) ? ChessColor.BLACK : ChessColor.WHITE;
            }

            chessboardModel.playerColor = playerPreferredColor;
            chessboardModel.opponentColor = (playerPreferredColor == ChessColor.BLACK) ? ChessColor.WHITE : ChessColor.BLACK;

            chessboardEventSignal.Dispatch(ChessboardEvent.GAME_STARTED);
        }
    }
}
