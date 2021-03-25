﻿using strange.extensions.command.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class AnalyseMoveCommand : Command
    {
        // Services
        [Inject] public IChessAiService chessAiService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        private Chessboard chessboard;

        public override void Execute()
        {
            Retain();

            chessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];

            var vo = new AiMoveInputVO();
            vo.aiColor = chessboard.playerColor;
            vo.playerColor = chessboard.opponentColor;
            vo.squares = chessboard.squares;
            vo.lastPlayerMove = chessboard.lastPlayerMove;
            vo.playerStrengthPct = 0.5f;
            vo.analyse = true;
            vo.fen = chessboard.fen;

            chessAiService.AnalyseMove(vo).Then(OnMoveAnalysed);
        }

        private void OnMoveAnalysed(FileRank from, FileRank to, string quality)
        {
            var bestMove = new ChessMove();
            bestMove.from = from;
            bestMove.to = to;
            bestMove.piece = chessboard.squares[from.file, from.rank].piece;

            var moveAnalysis = new MoveAnalysis();
            moveAnalysis.playerMove = chessboard.lastPlayerMove;
            moveAnalysis.bestMove = bestMove;
            moveAnalysis.moveQuality = MoveAnalysis.MoveQualityToEnum(quality);

            matchInfoModel.activeMatch.movesAnalysisList.Add(moveAnalysis);

            switch (moveAnalysis.moveQuality)
            {
                case MoveQuality.PERFECT:
                    matchInfoModel.activeMatch.matchAnalysis.perfectMoves++;
                    break;

                case MoveQuality.BLUNDER:
                    matchInfoModel.activeMatch.matchAnalysis.blunders++;
                    break;

                case MoveQuality.MISTAKE:
                    matchInfoModel.activeMatch.matchAnalysis.mistakes++;
                    break;
            }

            Release();
        }
    }
}