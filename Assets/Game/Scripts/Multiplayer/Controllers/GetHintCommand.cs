/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
///
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-16 13:27:47 UTC+05:00
///
/// @description
/// [add_description_here]

using strange.extensions.command.impl;

using TurboLabz.InstantFramework;
using strange.extensions.promise.api;
using TurboLabz.TLUtils;
using TurboLabz.Chess;

namespace TurboLabz.Multiplayer
{
    public class GetHintCommand : Command
    {
        // Parameters
        [Inject] public bool isHindsight { get; set; }

        // Dispatch Signals
        [Inject] public RenderHintSignal renderHintSignal { get; set; }

        // Services
        [Inject] public IChessAiService chessAiService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        Chessboard chessboard;

        public override void Execute()
        {
            Retain();
            chessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];

            string fen = isHindsight ? chessboard.previousPlayerTurnFen : chessboard.fen;

            chessAiService.SetPosition(fen);

            AiMoveInputVO vo = new AiMoveInputVO();
            vo.aiColor = chessboard.playerColor;
            vo.playerColor = chessboard.opponentColor;
            vo.squares = null;
            vo.aiMoveDelay = AiMoveDelay.NONE;
            vo.isHint = true;

            IPromise<FileRank, FileRank, string> promise = chessAiService.GetAiMove(vo);
            promise.Then(OnAiMove);
        }

        private void OnAiMove(FileRank from, FileRank to, string promo)
        {
            HintVO vo;
            vo.fromSquare = chessboard.squares[from.file, from.rank];
            vo.toSquare = chessboard.squares[to.file, to.rank];
            vo.availableHints = 0;
            renderHintSignal.Dispatch(vo);

            Release();
        }
    }
}
