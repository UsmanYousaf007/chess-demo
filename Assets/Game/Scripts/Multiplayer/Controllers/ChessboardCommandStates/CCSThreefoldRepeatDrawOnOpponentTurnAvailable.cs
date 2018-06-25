/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-20 14:49:17 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class CCSThreefoldRepeatDrawOnOpponentTurnAvailable : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (CameFromState(cmd, typeof(CCSOpponentTurn)))
            {
                RenderOpponentMove(cmd);
            }
            else if (CameFromState(cmd, typeof(CCSThreefoldRepeatDrawOnOpponentTurnAvailable)))
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG);
            }
            else if (CameFromState(cmd, typeof(CCSDefault)))
            {
                RenderNewGame(cmd, true);
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_THREEFOLD_REPEAT_DRAW_DLG);
            }        
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            ChessboardEvent evt = cmd.chessboardEvent;

            if (evt == ChessboardEvent.DRAW_CLAIMED)
            {   
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                cmd.claimThreefoldRepeatDrawSignal.Dispatch();
                return new CCSDrawClaimedOnOpponentTurn();
            }
            else if (evt == ChessboardEvent.DRAW_REJECTED)
            {
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);
                return new CCSPlayerTurn();
            }
            else if (evt == ChessboardEvent.OPPONENT_MOVE_RENDER_COMPLETED)
            {
                cmd.chessboardModel.opponentMoveRenderComplete = true;
                return new CCSThreefoldRepeatDrawOnOpponentTurnAvailable();
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