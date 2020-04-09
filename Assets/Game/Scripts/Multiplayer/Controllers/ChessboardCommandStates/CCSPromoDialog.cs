/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-21 16:08:31 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class CCSPromoDialog : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
            {
                if (!cmd.playerModel.autoPromotionToQueen)
                {
                    cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_PROMO_DLG);
                    Chessboard chessboard = cmd.activeChessboard;
                    cmd.hidePossibleMovesSignal.Dispatch();
                    cmd.updatePlayerPrePromoMoveSignal.Dispatch(GetMoveVO(chessboard, true));
                    cmd.updatePromoDialogSignal.Dispatch(cmd.activeChessboard.playerColor);
                }
                else
                {
                    Chessboard chessboard = cmd.activeChessboard;
                    cmd.hidePossibleMovesSignal.Dispatch();
                    cmd.updatePlayerPrePromoMoveSignal.Dispatch(GetMoveVO(chessboard, true));
                    cmd.autoQueenPromoSignal.Dispatch(cmd.activeChessboard.playerColor);
                }
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            ChessboardEvent evt = cmd.chessboardEvent;
            Chessboard chessboard = cmd.activeChessboard;

            if (evt == ChessboardEvent.PROMO_SELECTED)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return HandlePlayerMove(cmd, chessboard.selectedPromo);
            }
            else if (evt == ChessboardEvent.GAME_ENDED)
            {
                HandleGameEnded(cmd);
                return new CCSAnnounceResults();
            }

            return null;
        }
    }
}
