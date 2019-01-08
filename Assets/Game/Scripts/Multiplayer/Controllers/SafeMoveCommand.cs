/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential

using strange.extensions.command.impl;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class SafeMoveCommand : Command
    {
        // Parameters
        [Inject] public bool confirm { get; set; }

        // Dispatch Signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }


        public override void Execute()
        {
            chessboardModel.chessboards[matchInfoModel.activeChallengeId].inSafeMode = false;

            if (confirm)
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.MOVE_CONFIRMED);
            }
            else
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.MOVE_UNDO);
            }
        }
    }
}
