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
        [Inject] public CancelHintSingal cancelHintSignal { get; set; }
        [Inject] public ConsumeVirtualGoodSignal consumeVirtualGoodSignal { get; set; }
        [Inject] public UpdateHintCountSignal updateHintCountSignal { get; set; }
        [Inject] public UpdateHindsightCountSignal updateHindsightCountSignal { get; set; }

        // Services
        [Inject] public IChessAiService chessAiService { get; set; }

        // Models
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IPreferencesModel preferencesModel { get; set; }

        Chessboard chessboard;

        public override void Execute()
        { 
            
            chessboard = chessboardModel.chessboards[matchInfoModel.activeChallengeId];

            if (chessboard.lastPlayerMove != null)
            {
                Retain();

                AiMoveInputVO vo = new AiMoveInputVO();
                vo.aiColor = chessboard.playerColor;
                vo.playerColor = chessboard.opponentColor;
                vo.squares = chessboard.squares;
                vo.aiMoveDelay = AiMoveDelay.NONE;
                vo.lastPlayerMove = chessboard.lastPlayerMove;
                vo.isStrength = !isHindsight;
                vo.playerStrengthPct = 0.5f;
                vo.isHint = isHindsight;
                vo.fen = chessboard.previousPlayerTurnFen;

                IPromise<FileRank, FileRank, string> promise = chessAiService.GetAiMoveStrength(vo);
                promise.Then(OnAiMoveStrength);
            }
            else
            {
                LogUtil.Log("Required one move : ");
                cancelHintSignal.Dispatch();
            }

        }

        private void OnAiMoveStrength(FileRank from, FileRank to, string strength)
        {
            LogUtil.Log("OnAiMoveStrength : " + strength);

            HintVO newVo;
            newVo.fromSquare = chessboard.squares[from.file, from.rank];
            newVo.toSquare = chessboard.squares[to.file, to.rank];
            newVo.isHindsight = isHindsight;
            newVo.strength = -1;
            newVo.piece = "";
            newVo.skinId = playerModel.activeSkinId;
            newVo.didPlayerMadeBestMove = false;

            var chessMoveTemp = new ChessMove();
            if (chessMoveTemp.MoveToString(chessboard.lastPlayerMove.from, chessboard.lastPlayerMove.to).Equals(chessMoveTemp.MoveToString(from, to)))
            {
                newVo.didPlayerMadeBestMove = true;
            }

            if (isHindsight)
            {
                var pieceColor = strength[0].Equals('b') ? ChessColor.BLACK : ChessColor.WHITE;

                //check if piece color is of opponent's then player's piece is captured
                if (pieceColor != chessboard.playerColor)
                {
                    //set captured piece flag
                    strength = string.Format("{0}captured", chessboard.playerColor == ChessColor.BLACK ? 'b' : 'W');
                }

                if (!string.IsNullOrEmpty(chessboard.lastPlayerMove.promo)
                    && newVo.didPlayerMadeBestMove)
                {
                    strength = string.Format("{0}p", chessboard.playerColor == ChessColor.BLACK ? 'b' : 'W');
                }

                newVo.piece = strength;
            }
            else
            {
                var piece = chessboard.squares[to.file, to.rank].piece;
                var pieceName = piece.name;
                pieceName = string.Format("{0}{1}", piece.color == ChessColor.BLACK ? 'b' : 'W', pieceName.ToLower());
                if (piece.color != chessboard.playerColor)
                {
                    pieceName = string.Format("{0}captured", chessboard.playerColor == ChessColor.BLACK ? 'b' : 'W');
                }

                if (!string.IsNullOrEmpty(chessboard.lastPlayerMove.promo))
                {
                    pieceName = string.Format("{0}p", chessboard.playerColor == ChessColor.BLACK ? 'b' : 'W');
                }

                newVo.piece = pieceName;
                newVo.strength = float.Parse(strength);
            }

            renderHintSignal.Dispatch(newVo);

            if (isHindsight)
            {
                updateHindsightCountSignal.Dispatch(playerModel.PowerUpHindsightCount - 1);
                consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.HINDSIGHT, 1);
                preferencesModel.isCoachTooltipShown = true;
            }
            else
            {
                updateHintCountSignal.Dispatch(playerModel.PowerUpHintCount - 1);
                consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.HINT, 1);
                preferencesModel.isStrengthTooltipShown = true;
            }

            Release();
        }

        private void OnAiMove(FileRank from, FileRank to, string promo)
        {
            HintVO vo;
            vo.fromSquare = chessboard.squares[from.file, from.rank];
            vo.toSquare = chessboard.squares[to.file, to.rank];
            vo.isHindsight = isHindsight;
            vo.strength = 0;
            vo.piece = "";
            vo.skinId = playerModel.activeSkinId;
            vo.didPlayerMadeBestMove = false;
            renderHintSignal.Dispatch(vo);

            if (isHindsight)
            {
                updateHindsightCountSignal.Dispatch(playerModel.PowerUpHindsightCount - 1);
                consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.HINDSIGHT, 1);
            }
            else
            {
                updateHintCountSignal.Dispatch(playerModel.PowerUpHintCount - 1);
                consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.HINT, 1);
            }

            Release();
        }
    }
}
