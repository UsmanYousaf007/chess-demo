/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Mubeen Iqbal <mubeen@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-05-18 16:40:43 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;
using GameSparks.Core;
using TurboLabz.Chess;
using TurboLabz.Multiplayer;

namespace TurboLabz.InstantFramework
{
    public partial class GSService
    {
        private void InitGame(GSData gameData, string challengeId)
        {
            LoadChessboardModel(gameData, challengeId);
        }

        private void UpdateLongPlayGame(GSData gameData, string challengeId)
        {
            LoadChessboardModel(gameData, challengeId);

            Chessboard chessboard = chessboardModel.chessboards[challengeId];

            // Load up the backend moves
            IList<GSData> backendMoveList = gameData.GetGSDataList(GSBackendKeys.MOVE_LIST);
            chessboard.moveList = new List<ChessMove>();

            foreach (GSData data in backendMoveList)
            {
                ChessMove move = new ChessMove();
                string fromSquareStr = data.GetString(GSBackendKeys.FROM_SQUARE);
                string toSquareStr = data.GetString(GSBackendKeys.TO_SQUARE);

                move.from = chessService.GetFileRankLocation(fromSquareStr[0], fromSquareStr[1]);
                move.to = chessService.GetFileRankLocation(toSquareStr[0], toSquareStr[1]);
                move.promo = data.GetString(GSBackendKeys.PROMOTION);
                chessboard.moveList.Add(move);
            }
        }
    }
}
