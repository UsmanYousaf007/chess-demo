/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using System.Collections.Generic;
using strange.extensions.command.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.CPU
{
    public class SafeMoveCommand : Command
    {
        // Parameters
        [Inject] public bool confirm { get; set; }

        // Dispatch Signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public StopTimersSignal stopTimersSignal { get; set; }
        [Inject] public UpdateSafeMoveCountSignal updateSafeMoveCountSignal { get; set; }
        [Inject] public ConsumeVirtualGoodSignal consumeVirtualGoodSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        // Services
        [Inject] public IAnalyticsService analyticsService { get; set; }


        public override void Execute()
        {
            if (confirm)
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.MOVE_CONFIRMED);
            }
            else
            {
                // Trim the last move from the model
                List<ChessMove> moveList = chessboardModel.moveList;
                moveList.RemoveRange(moveList.Count - 1, 1);


                // Reset the notation stored in the model
                chessboardModel.notation = new List<string>();
                chessboardModel.usedHelp = true;

                // Stop the running timer
                stopTimersSignal.Dispatch();

                // Store the fact that we are undoing
                chessboardModel.isUndo = true;

                chessboardEventSignal.Dispatch(ChessboardEvent.MOVE_UNDO);


                UpdateSafeMoveCounts();


                analyticsService.Event(AnalyticsEventId.tap_pow_safe_move_undo, AnalyticsContext.computer_match);
            }
        }

        void UpdateSafeMoveCounts()
        {
            updateSafeMoveCountSignal.Dispatch(playerModel.PowerUpSafeMoveCount - 1);

            if (playerModel.PowerUpSafeMoveCount - 1 == 0)
            {
                chessboardModel.inSafeMode = false;
            }

            consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.SAFE_MOVE, 1);
        }
    }
}
