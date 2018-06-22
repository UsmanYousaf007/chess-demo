/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-21 15:56:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class CCSAnnounceResults : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (CameFromState(cmd, typeof(CCSPromoDialog)))
            {
                cmd.hidePromoDialogSignal.Dispatch();
            }
            else if (CameFromState(cmd, typeof(CCSFiftyMoveDrawOnOpponentTurnAvailable)) ||
                     CameFromState(cmd, typeof(CCSFiftyMoveDrawOnOpponentTurnAvailablePieceSelected)) ||
                     CameFromState(cmd, typeof(CCSFiftyMoveDrawOnPlayerTurnAvailable)) ||
                     CameFromState(cmd, typeof(CCSThreefoldRepeatDrawOnOpponentTurnAvailable)) ||
                     CameFromState(cmd, typeof(CCSThreefoldRepeatDrawOnOpponentTurnAvailablePieceSelected)) ||
                     CameFromState(cmd, typeof(CCSThreefoldRepeatDrawOnPlayerTurnAvailable))) 
            {
                cmd.hideDrawDialogSignal.Dispatch();
            } 

            IChessboardModel model = cmd.chessboardModel;
            bool playerWins = (model.winnerId == cmd.playerModel.id) ? true : false;

            cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_RESULTS_DLG);
            cmd.updateResultsDialogSignal.Dispatch(cmd.chessboardModel.gameEndReason, playerWins);
        }
    }
}
