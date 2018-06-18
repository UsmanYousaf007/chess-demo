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
using System.Collections.Generic;
using TurboLabz.Chess;
using TurboLabz.TLUtils;


namespace TurboLabz.InstantChess
{
    public class UndoMoveCommand : Command
    {
        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }

        // Models
        [Inject] public ICPUChessboardModel chessboardModel { get; set; }

        public override void Execute()
        {
            // Trim the last 2 moves from the model
            List<ChessMove> moveList = chessboardModel.moveList;
            moveList.RemoveRange(moveList.Count - 2, 2);

            // Reset the notation stored in the model
            chessboardModel.notation = new List<string>();
            chessboardModel.usedHelp = true;

            // Stop the running timer
            stopTimersSignal.Dispatch();

            // Store the fact that we are undoing
            chessboardModel.isUndo = true;

            chessboardEventSignal.Dispatch(ChessboardEvent.MOVE_UNDO);
        }
    }
}
