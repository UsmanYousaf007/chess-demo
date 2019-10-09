/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-17 14:45:12 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;

namespace TurboLabz.CPU
{
    public class ChessboardSquareClickedCommand : Command
    {
        // Parameters
        [Inject] public FileRank clickedLocation { get; set; }

        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }

        public override void Execute()
        {
            // If not an offboard tap
            if (clickedLocation.file != -1)
            {
                chessboardModel.clickedSquare = chessboardModel.squares[clickedLocation.file, clickedLocation.rank];
                chessboardEventSignal.Dispatch(ChessboardEvent.SQUARE_CLICKED);
            }
            else
            {
                chessboardEventSignal.Dispatch(ChessboardEvent.OFFBOARD_CLICKED);
            }
        }

    }
}
