using strange.extensions.command.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using UnityEngine;
using System.Linq;

namespace TurboLabz.Multiplayer
{
    public class AnalyseMoveCommand : Command
    {
        //Parameters
        [Inject] public AnalyseMoveParameters parameters { get; set; }

        // Services
        [Inject] public IChessAiService chessAiService { get; set; }
        [Inject] public IChessService chessService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }

        // Dispatch Signals
        [Inject] public MoveAnalysiedSignal moveAnalysiedSignal { get; set; }

        private Chessboard chessboard;

        public override void Execute()
        {
            if (chessboardModel.chessboards.ContainsKey(parameters.challengeId))
            {
                Retain();
                var moveAnalysis = new MoveAnalysis();
                moveAnalysis.playerMove = parameters.chessMove;
                moveAnalysiedSignal.Dispatch(parameters.challengeId, moveAnalysis, null);

                chessboard = chessboardModel.chessboards[parameters.challengeId];

                var vo = new AiMoveInputVO();
                vo.aiColor = chessboard.playerColor;
                vo.playerColor = chessboard.opponentColor;
                vo.squares = chessboard.squares;
                vo.lastPlayerMove = parameters.isLastTurn ? new ChessMove(true) : parameters.chessMove;
                vo.playerStrengthPct = 0.5f;
                vo.analyse = true;
                vo.fen = parameters.isPlayerTurn ? chessboard.fen : chessService.GetFen();

                chessAiService.AnalyseMove(vo).Then(OnMoveAnalysed);
            }
        }

        private void OnMoveAnalysed(FileRank from, FileRank to, string analysis)
        {
            var parsedAnalysis = analysis.Split('|');
            var quality = parsedAnalysis[0];
            var strength = float.Parse(parsedAnalysis[1]);

            var bestMove = new ChessMove();
            bestMove.from = from;
            bestMove.to = to;
            bestMove.piece = chessboard.squares[from.file, from.rank].piece;

            var moveAnalysis = new MoveAnalysis();
            moveAnalysis.playerMove = parameters.chessMove;
            moveAnalysis.bestMove = bestMove;
            moveAnalysis.moveQuality = MoveAnalysis.MoveQualityToEnum(quality);
            moveAnalysis.strength = strength;
            moveAnalysis.isPlayerMove = parameters.isPlayerTurn;
            moveAnalysis.advantageScore = int.Parse(parsedAnalysis[2]);
            moveAnalysis.playerScore = int.Parse(parsedAnalysis[3]);
            moveAnalysis.bestScore = int.Parse(parsedAnalysis[4]);

            var matchInfo = matchInfoModel.matches.ContainsKey(parameters.challengeId) ? matchInfoModel.matches[parameters.challengeId] :
                matchInfoModel.lastCompletedMatch.challengeId.Equals(parameters.challengeId) ? matchInfoModel.lastCompletedMatch : null;

            if (matchInfo != null)
            {
                if (matchInfo.movesAnalysisList != null)
                {
                    if (matchInfo.matchAnalysis != null && parameters.isPlayerTurn && !parameters.isLastTurn)
                    {
                        switch (moveAnalysis.moveQuality)
                        {
                            case MoveQuality.PERFECT:
                                matchInfo.matchAnalysis.perfectMoves++;
                                break;

                            case MoveQuality.BLUNDER:
                                matchInfo.matchAnalysis.blunders++;
                                break;

                            case MoveQuality.MISTAKE:
                                matchInfo.matchAnalysis.mistakes++;
                                break;
                        }
                    }

                    if (matchInfo.movesAnalysisList.Count == 0)
                    {
                        moveAnalysis.playerAdvantage = 0.0f;
                    }
                    else
                    {
                        var lastMove = matchInfo.movesAnalysisList.Last();

                        parameters.isPlayerTurn = parameters.isLastTurn ? !lastMove.isPlayerMove : parameters.isPlayerTurn;

                        lastMove.playerAdvantage = (parameters.isPlayerTurn ?
                             (moveAnalysis.bestScore - lastMove.advantageScore) :
                             (lastMove.advantageScore - moveAnalysis.bestScore)) / 100.0f;

                        lastMove.playerAdvantage = Mathf.Clamp(lastMove.playerAdvantage, -10.0f, 10.0f);
                        moveAnalysis.playerAdvantage = lastMove.playerAdvantage;

                        moveAnalysiedSignal.Dispatch(parameters.challengeId, lastMove, matchInfo.matchAnalysis);
                    }

                    if (!parameters.isLastTurn)
                    {
                        matchInfo.movesAnalysisList.Add(moveAnalysis);
                    }
                }

                
            }

            Release();
        }
    }
}
