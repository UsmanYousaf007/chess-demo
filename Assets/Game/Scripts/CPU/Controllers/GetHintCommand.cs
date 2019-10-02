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
using System.Collections.Generic;

namespace TurboLabz.CPU
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
        [Inject] public IChessService chessService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }
        [Inject] public IPlayerModel playerModel { get; set; }

        public override void Execute()
        {
            if(chessboardModel.lastPlayerMove != null)
            {
                Retain();

                chessboardModel.usedHelp = true;

                //string fen = isHindsight ? chessboardModel.previousPlayerTurnFen : chessService.GetFen();
                chessAiService.NewGame();
                chessAiService.SetPosition(chessboardModel.previousPlayerTurnFen);

                AiMoveInputVO vo = new AiMoveInputVO
                {
                    aiColor = chessboardModel.playerColor,
                    playerColor = chessboardModel.opponentColor,
                    squares = chessboardModel.squares,
                    aiMoveDelay = AiMoveDelay.NONE,
                    lastPlayerMove = chessboardModel.lastPlayerMove,
                    isStrength = !isHindsight,
                    playerStrengthPct = 0.5f,
                    isHint = isHindsight
                };

                //IPromise<FileRank, FileRank, string> promise = chessAiService.GetAiMove(vo);
                //promise.Then(OnAiMove);
                
                IPromise<FileRank, FileRank, string> promise1 = chessAiService.GetAiMoveStrength(vo);
                promise1.Then(OnAiMoveStrength);

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
            newVo.fromSquare = chessboardModel.squares[from.file, from.rank];
            newVo.toSquare = chessboardModel.squares[to.file, to.rank];
            newVo.isHindsight = isHindsight;
            newVo.strength = -1;
            newVo.piece = "";
            newVo.skinId = playerModel.activeSkinId;
            newVo.didPlayerMadeBestMove = false;

            var chessMoveTemp = new ChessMove();
            if (chessMoveTemp.MoveToString(chessboardModel.lastPlayerMove.from, chessboardModel.lastPlayerMove.to).Equals(chessMoveTemp.MoveToString(from, to)))
            {
                newVo.didPlayerMadeBestMove = true;
            }

            if (isHindsight)
            {
                var pieceColor = strength[0].Equals('b') ? ChessColor.BLACK : ChessColor.WHITE;

                //check if piece color is of opponent's then player's piece is captured
                if (pieceColor != chessboardModel.playerColor)
                {
                    //set captured piece flag
                    strength = string.Format("{0}captured", chessboardModel.playerColor == ChessColor.BLACK ? 'b' : 'W');
                }

                newVo.piece = strength;
            }
            else
            {
                newVo.strength = int.Parse(strength);
            }
            
            renderHintSignal.Dispatch(newVo);

            //if (isHindsight)
            //{
            //    updateHindsightCountSignal.Dispatch(playerModel.PowerUpHindsightCount - 1);
            //    consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.HINDSIGHT, 1);
            //}
            //else
            //{
            //    updateHintCountSignal.Dispatch(playerModel.PowerUpHintCount - 1);
            //    consumeVirtualGoodSignal.Dispatch(GSBackendKeys.PowerUp.HINT, 1);
            //}

            Release();
        }


        private void OnAiMove(FileRank from, FileRank to, string promo)
        {
            HintVO vo;
            vo.fromSquare = chessboardModel.squares[from.file, from.rank];
            vo.toSquare = chessboardModel.squares[to.file, to.rank];
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
