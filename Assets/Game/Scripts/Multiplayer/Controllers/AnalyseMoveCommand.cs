using strange.extensions.command.impl;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;

namespace TurboLabz.Multiplayer
{
    public class AnalyseMoveCommand : Command
    {
        //Parameters
        [Inject] public ChessMove chessMove { get; set; }
        [Inject] public bool isPlayerTurn { get; set; }

        // Services
        [Inject] public IChessAiService chessAiService { get; set; }
        [Inject] public IChessService chessService { get; set; }

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
            vo.lastPlayerMove = chessMove;
            vo.playerStrengthPct = 0.5f;
            vo.analyse = true;
            vo.fen = isPlayerTurn ? chessboard.fen : chessService.GetFen();

            chessAiService.AnalyseMove(vo).Then(OnMoveAnalysed);
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
            moveAnalysis.playerMove = chessMove;
            moveAnalysis.bestMove = bestMove;
            moveAnalysis.moveQuality = MoveAnalysis.MoveQualityToEnum(quality);
            moveAnalysis.strength = strength;
            moveAnalysis.isPlayerMove = isPlayerTurn;
            moveAnalysis.whiteScore = chessService.GetScore(ChessColor.WHITE);
            moveAnalysis.blackScore = chessService.GetScore(ChessColor.BLACK);

            if (matchInfoModel.activeMatch != null)
            {
                if (matchInfoModel.activeMatch.movesAnalysisList != null)
                {
                    matchInfoModel.activeMatch.movesAnalysisList.Add(moveAnalysis);
                }

                if (matchInfoModel.activeMatch.matchAnalysis != null && isPlayerTurn)
                {
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
                }
            }

            Release();
        }
    }
}
