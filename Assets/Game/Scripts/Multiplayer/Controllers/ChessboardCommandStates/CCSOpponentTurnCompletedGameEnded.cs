/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-21 16:04:29 UTC+05:00
/// 
/// @description
/// [add_description_here]

using TurboLabz.Chess;

namespace TurboLabz.Multiplayer
{
    public class CCSOpponentTurnCompletedGameEnded : CCS
    {
        public override void RenderDisplayOnEnter(ChessboardCommand cmd)
        {
            if (CameFromState(cmd, typeof(CCSOpponentTurn)) ||
                CameFromState(cmd, typeof(CCSOpponentTurnPieceSelected)))
            {
                RenderOpponentMove(cmd);
            }
        }

        public override CCS HandleEvent(ChessboardCommand cmd)
        {
            ChessboardEvent evt = cmd.chessboardEvent;

            if (evt == ChessboardEvent.OPPONENT_MOVE_RENDER_COMPLETED)
            {
                OpponentMoveRenderCompleted(cmd);
                HandleGameEnded(cmd);
                return new CCSAnnounceResults();
            }

            return null;
        }
    }
}
