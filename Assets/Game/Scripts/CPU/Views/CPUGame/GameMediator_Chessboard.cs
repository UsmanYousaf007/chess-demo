/// @license Propriety <http://license.url>
/// @copyright Copyright (C) Turbo Labz 2017 - All rights reserved
/// Unauthorized copying of this file, via any medium is strictly prohibited
/// Proprietary and confidential
/// 
/// @author Faraz Ahmed <faraz@turbolabz.com>
/// @company Turbo Labz <http://turbolabz.com>
/// @date 2017-01-06 22:01:38 UTC+05:00
/// 
/// @description
/// [add_description_here]

using System.Collections.Generic;
using TurboLabz.Chess;

namespace TurboLabz.CPUChess
{
    public partial class GameMediator
    {
        // Dispatch signals
        [Inject] public SquareClickedSignal squareClickedSignal { get; set; }

        public void OnRegisterChessboard()
        {
            view.InitChessboard();
            view.squareClickedSignal.AddListener(OnSquareClicked);
            view.opponentMoveRenderComplete.AddListener(OnOpponentMoveRenderComplete);
        }

        public void OnRemoveChessboard()
        {
            view.RemoveChessboardListeners();
            view.squareClickedSignal.RemoveListener(OnSquareClicked);
            view.opponentMoveRenderComplete.RemoveListener(OnOpponentMoveRenderComplete);
        }

        [ListensTo(typeof(SetupChessboardSignal))]
        public void OnSetupChessboard(bool isPlayerTurn)
        {
            view.SetupChessboard(isPlayerTurn);
        }

        [ListensTo(typeof(ShowPossibleMovesSignal))]
        public void OnUpdatePossibleMoves(FileRank pieceLocation, List<ChessSquare> possibleMoves)
        {
            view.ShowPossibleMoves(pieceLocation, possibleMoves);
        }

        [ListensTo(typeof(HidePossibleMovesSignal))]
        public void OnHidePossibleMoves()
        {
            view.HidePossibleMoves();
        }

        [ListensTo(typeof(UpdateChessboardSignal))]
        public void OnUpdateChessboard(ChessSquare[,] chessSquares)
        {
            view.UpdateChessboard(chessSquares);
        }

        [ListensTo(typeof(UpdatePlayerMoveSignal))]
        public void OnUpdatePlayerMoveSignal(MoveVO moveVO)
        {
            view.UpdatePlayerMove(moveVO);
        }

        [ListensTo(typeof(UpdatePlayerPrePromoMoveSignal))]
        public void OnUpdatePlayerPrePromoMoveSignal(MoveVO vo)
        {
            view.UpdatePlayerPrePromoMove(vo);
        }

        [ListensTo(typeof(UpdatePromoSignal))]
        public void OnUpdatePromo(MoveVO moveVO)
        {
            view.UpdatePiecesPostPromo(moveVO);
        }

        [ListensTo(typeof(UpdateOpponentMoveSignal))]
        public void OnUpdateOpponentMove(MoveVO moveVO)
        {
            view.UpdateOpponentMove(moveVO);
        }

        [ListensTo(typeof(ShowPlayerFromIndicatorSignal))]
        public void OnShowPlayerFromIndicator(ChessSquare square)
        {
            view.ShowPlayerFromIndicator(square);
        }

        [ListensTo(typeof(ShowPlayerToIndicatorSignal))]
        public void OnShowPlayerToIndicator(ChessSquare square)
        {
            view.ShowPlayerToIndicator(square);
        }

        [ListensTo(typeof(HidePlayerFromIndicatorSignal))]
        public void OnHidePlayerFromIndicator()
        {
            view.HidePlayerFromIndicator();
        }

        [ListensTo(typeof(HidePlayerToIndicatorSignal))]
        public void OnHidePlayerToIndicator()
        {
            view.HidePlayerToIndicator();
        }

        [ListensTo(typeof(ShowOpponentFromIndicatorSignal))]
        public void OnShowOpponentFromIndicator(ChessSquare square)
        {
            view.ShowOpponentFromIndicator(square);
        }

        [ListensTo(typeof(ShowOpponentToIndicatorSignal))]
        public void OnShowOpponentToIndicator(ChessSquare square)
        {
            view.ShowOpponentToIndicator(square);
        }

        [ListensTo(typeof(HideOpponentFromIndicatorSignal))]
        public void OnHideOpponentFromIndicator()
        {
            view.HideOpponentFromIndicator();
        }

        [ListensTo(typeof(HideOpponentToIndicatorSignal))]
        public void OnHideOpponentToIndicator()
        {
            view.HideOpponentToIndicator();
        }

        [ListensTo(typeof(UpdateMoveForResumeSignal))]
        public void OnUpdateMoveForResume(MoveVO vo, bool isPlayerTurn)
        {
            view.UpdateMoveForResume(vo, isPlayerTurn);
        }

        private void OnSquareClicked(FileRank fileRank)
        {
            squareClickedSignal.Dispatch(fileRank);
        }

        private void OnOpponentMoveRenderComplete()
        {
            chessboardEventSignal.Dispatch(ChessboardEvent.OPPONENT_MOVE_RENDER_COMPLETED);
        }
    }
}
