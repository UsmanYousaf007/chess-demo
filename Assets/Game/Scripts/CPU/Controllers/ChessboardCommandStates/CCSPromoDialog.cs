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

namespace TurboLabz.CPU
{
    public class CCSPromoDialog : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
            {
                if(!cmd.preferencesModel.autoPromotionToQueen)
                {
                    cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_PROMO_DLG);

                    IChessboardModel model = cmd.chessboardModel;
                    cmd.hidePossibleMovesSignal.Dispatch();
                    cmd.updatePlayerPrePromoMoveSignal.Dispatch(GetMoveVO(model, true));
                    cmd.updatePromoDialogSignal.Dispatch(cmd.chessboardModel.playerColor);
                }
                else
                {
                    IChessboardModel model = cmd.chessboardModel;
                    cmd.hidePossibleMovesSignal.Dispatch();
                    cmd.updatePlayerPrePromoMoveSignal.Dispatch(GetMoveVO(model, true));
                    cmd.autoQueenPromoSignal.Dispatch(cmd.chessboardModel.playerColor);
                }
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            ChessboardEvent evt = cmd.chessboardEvent;
            IChessboardModel model = cmd.chessboardModel;

            if (evt == ChessboardEvent.PROMO_SELECTED)
            {
                return DoPlayerMove(cmd, model.selectedPromo);
            }
            else if (evt == ChessboardEvent.GAME_ENDED)
            {
                ProcessGameEndTimers(cmd);
                return new CCSAnnounceResults();
            }

            return null;
        }
    }
}
