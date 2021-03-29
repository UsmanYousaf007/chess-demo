/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-02-18 18:17:00 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class PlayerTurnCommand : Command
    {
        // Parameters
        [Inject] public PlayerTurnVO playerTurnVO { get; set; }

        // Dispatch Signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }
        [Inject] public CancelHintSingal cancelHintSignal { get; set; }
        [Inject] public SyncReconnectDataSignal syncReconnectDataSignal { get; set; }
        [Inject] public AnalyseMoveSignal analyseMoveSignal { get; set; }

        // Services
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        private string challengeId = null;

        public override void Execute()
        {
            Retain();

            //cancelHintSignal.Dispatch();
            challengeId = matchInfoModel.activeChallengeId;

            backendService.PlayerTurn(
                playerTurnVO.fromSquare.fileRank,
                playerTurnVO.toSquare.fileRank,
                playerTurnVO.promo,
                playerTurnVO.claimFiftyMoveDraw,
                playerTurnVO.claimThreefoldRepeatDraw,
                playerTurnVO.rejectThreefoldRepeatDraw).Then(OnTurnTaken); 

            SaveLastPlayerMove();
        }

        private void OnTurnTaken(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                if (challengeId == matchInfoModel.activeChallengeId)
                {
                    syncReconnectDataSignal.Dispatch(challengeId);
                }
            }

            Release();
        }

        // We need to save player ChessMove's for the Ai engine to analyse
        private void SaveLastPlayerMove()
        {
            ChessMove move = new ChessMove();
            move.from = playerTurnVO.fromSquare.fileRank;
            move.to = playerTurnVO.toSquare.fileRank;
            move.piece = playerTurnVO.fromSquare.piece;
            move.promo = playerTurnVO.promo;

            Chessboard chessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];
            chessboard.lastPlayerMove = move;

            analyseMoveSignal.Dispatch(move, true);
        }
    }
}
