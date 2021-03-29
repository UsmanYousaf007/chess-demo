using System.Collections.Generic;
using strange.extensions.command.impl;
using TurboLabz.Chess;

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

        public override void Execute()
        {
            var squares = new ChessSquare[8, 8];
            chessService.NewGame(squares);

            foreach (var move in moves)
            {
                chessService.MakeMove(move.playerMove.from, move.playerMove.to, move.playerMove.promo, move.isPlayerMove, squares);
            }

            updateChessboardSignal.Dispatch(squares);
        }
    }
}
