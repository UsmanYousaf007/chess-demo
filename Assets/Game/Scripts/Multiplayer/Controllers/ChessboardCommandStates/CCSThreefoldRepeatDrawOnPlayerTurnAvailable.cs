/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-20 16:17:44 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class CCSThreefoldRepeatDrawOnPlayerTurnAvailable : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (CameFromState(cmd, typeof(CCSPlayerTurnPieceSelected)))
            {
                RenderPlayerMove(cmd);
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG);
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            IChessboardModel model = cmd.chessboardModel;
            ChessboardEvent evt = cmd.chessboardEvent;

            if (cmd.chessboardEvent == ChessboardEvent.DRAW_REJECTED)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                SendPlayerTurn(cmd, model.playerMoveFlag, false, false, true, false);
                return new CCSOpponentTurn();
            }
            else if (cmd.chessboardEvent == ChessboardEvent.DRAW_CLAIMED)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                SendPlayerTurn(cmd, model.playerMoveFlag, false, true, false, false);
                return new CCSPlayerTurnCompletedGameEnded();
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
