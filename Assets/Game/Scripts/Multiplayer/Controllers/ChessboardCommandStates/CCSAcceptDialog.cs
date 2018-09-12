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
    public class CCSAcceptDialog : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER_ACCEPT_DLG);
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            ChessboardEvent evt = cmd.chessboardEvent;

            if (evt == ChessboardEvent.GAME_ACCEPTED)
            {
                cmd.acceptSignal.Dispatch(cmd.matchInfoModel.activeChallengeId);
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);

                if (cmd.activeChessboard.isPlayerTurn)
                {
                    return new CCSPlayerTurn();
                }
                else
                {
                    return new CCSOpponentTurn();
                }
            }
            else if (evt == ChessboardEvent.GAME_DECLINED)
            {
                string challengeId = cmd.matchInfoModel.activeChallengeId;
                cmd.declineSignal.Dispatch(challengeId);
                cmd.unregisterSignal.Dispatch(challengeId);
                cmd.navigatorEventSignal.Dispatch(NavigatorEvent.SHOW_MULTIPLAYER);

                return null;
            }
            else if (evt == ChessboardEvent.GAME_ENDED)
            {
                HandleGameEnded(cmd);
                return new CCSAnnounceResults();
            }
            // We received an opponent moved event from the backend service
            else if (evt == ChessboardEvent.OPPONENT_MOVE_COMPLETE)
            {
                HandleOpponentBackendMoved(cmd);
                return new CCSPlayerTurn();
            }

            return null;
        }
    }
}
