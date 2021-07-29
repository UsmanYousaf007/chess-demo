using System.Collections.Generic;
using strange.extensions.command.impl;
using TurboLabz.Chess;
using GameAnalyticsSDK;

namespace TurboLabz.Multiplayer
{
    public class RenderMoveAnalysisCommand : Command
    {
        //Parameters
        [Inject] public List<MoveAnalysis> moves { get; set; }

        // Services
        [Inject] public IChessService chessService { get; set; }

        //Dispatch Signals
        [Inject] public UpdateChessboardSignal updateChessboardSignal { get; set; }
        [Inject] public UpdateKingCheckIndicatorSignal updateKingCheckIndicatorSignal { get; set; }

        public override void Execute()
        {
            var lastMoveResult = new ChessMoveResult();
            var squares = new ChessSquare[8, 8];
            chessService.NewGame(squares);

            foreach (var move in moves)
            {
                try
                {
                    lastMoveResult = chessService.MakeMove(move.playerMove.from, move.playerMove.to, move.playerMove.promo, move.isPlayerMove, squares);
                }
                catch (System.Exception ex)
                {
                    if (move.playerMove.from == null)
                    {
                        GameAnalytics.NewErrorEvent(GAErrorSeverity.Debug, "RenderMoveAnalysisCommand.move.playerMove.from is null");
                    }
                    else if (move.playerMove.to == null)
                    {
                        GameAnalytics.NewErrorEvent(GAErrorSeverity.Debug, "RenderMoveAnalysisCommand.move.playerMove.to is null");
                    }
                    else if (move.playerMove.promo == null)
                    {
                        GameAnalytics.NewErrorEvent(GAErrorSeverity.Debug, "RenderMoveAnalysisCommand.move.playerMove.promo is null");
                    }
                    else
                    {
                        GameAnalytics.NewErrorEvent(GAErrorSeverity.Debug, "RenderMoveAnalysisCommand.something is null");
                    }
                }
            }

            var moveVO = new MoveVO();
            moveVO.isPlayerInCheck = lastMoveResult.isPlayerInCheck;
            moveVO.isOpponentInCheck = lastMoveResult.isOpponentInCheck;

            updateChessboardSignal.Dispatch(squares);
            updateKingCheckIndicatorSignal.Dispatch(moveVO);
        }
    }
}
