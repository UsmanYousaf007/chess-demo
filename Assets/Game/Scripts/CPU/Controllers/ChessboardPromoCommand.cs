/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-27 14:24:22 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;

namespace TurboLabz.InstantChess
{
    public class ChessboardPromoCommand : Command
    {
        // Parameters
        [Inject] public string pieceName { get; set; }

        // Dispatch signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Models
        [Inject] public ICPUChessboardModel chessboardModel { get; set; }

        public override void Execute()
        {
            chessboardModel.selectedPromo = pieceName;
            chessboardEventSignal.Dispatch(ChessboardEvent.PROMO_SELECTED);
        }

    }
}
