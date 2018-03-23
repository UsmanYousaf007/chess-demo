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
using TurboLabz.TLUtils;
using TurboLabz.InstantFramework;

namespace TurboLabz.InstantChess
{
    public class CCSAnnounceResults : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (GameEndHasMove(cmd))
            {
                if (CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
                {
                    RenderPlayerMove(cmd);
                }
                else if (CameFromState(cmd, typeof(CCSPromoDialog)))
                {
                    RenderPromo(cmd);
                } 
                else if (CameFromState(cmd, typeof(CCSOpponentTurn)) ||  
                         CameFromState(cmd, typeof(CCSOpponentTurnPieceSelected)))
                {
                    RenderOpponentMove(cmd);
                }
            }

            IChessboardModel model = cmd.chessboardModel;
            bool playerWins = (model.winnerId == cmd.playerModel.id) ? true : false;

            cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_CPU_PLAY_RESULTS_DLG);
            cmd.updateResultsDialogSignal.Dispatch(model.gameEndReason, playerWins);


            cmd.disableUndoButtonSignal.Dispatch();
            cmd.disableMenuButtonSignal.Dispatch();
            cmd.disableHintButtonSignal.Dispatch();

            cmd.cpuGameModel.inProgress = false;
            cmd.saveGameSignal.Dispatch();


        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            return null;
        }

        private bool GameEndHasMove(ChessboardCommand cmd)
        {
            GameEndReason reason = cmd.chessboardModel.gameEndReason;

            return (reason == GameEndReason.CHECKMATE ||
                    reason == GameEndReason.STALEMATE ||
                    reason == GameEndReason.DRAW_BY_INSUFFICIENT_MATERIAL);
        }
    }
}
