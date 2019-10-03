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
using System.Collections;
using UnityEngine;

namespace TurboLabz.CPU
{
    public class AiTurnCommand : Command
    {
        // Dispatch Signals
        [Inject] public ChessboardEventSignal chessboardEventSignal { get; set; }

        // Services
        [Inject] public IChessService chessService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }

        // Models
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public ICPUGameModel cpuGameModel { get; set; }

        // Utils
        [Inject] public IRoutineRunner routineRunner { get; set; }

        float startTime;

        public override void Execute()
        {
            Retain();
            startTime = Time.time;

            ++chessboardModel.aiMoveNumber;
            chessAiService.SetPosition(chessService.GetFen());

            AiMoveInputVO vo = new AiMoveInputVO();
            vo.aiColor = chessboardModel.opponentColor;
            vo.playerColor = chessboardModel.playerColor;
            vo.lastPlayerMove = chessboardModel.lastPlayerMove;
            vo.squares = chessboardModel.squares;
            vo.opponentTimer = chessboardModel.opponentTimer;
            vo.aiMoveDelay = chessboardModel.aiMoveDelay;
            vo.aiMoveNumber = chessboardModel.aiMoveNumber;
            vo.cpuStrengthPct = (float)cpuGameModel.cpuStrength / (float)CPUSettings.MAX_STRENGTH;
           
            double durationMins = chessboardModel.gameDuration.TotalMinutes;

            // Now adjust for strength based on time control
            if (durationMins > 0)
            {
                if (durationMins <= ChessAiConfig.DURATION_FAST_MINS)
                {
                    vo.cpuStrengthPct *= ChessAiConfig.FAST_GIMP;
                }
                else if (durationMins <= ChessAiConfig.DURATION_MEDIUM_MINS)
                {
                    vo.cpuStrengthPct *= ChessAiConfig.MEDIUM_GIMP;
                }
            }

            IPromise<FileRank, FileRank, string> promise = chessAiService.GetAiMove(vo);

            if (promise == null)
            {
                throw new System.Exception("promise is null");
            }

            promise.Then(OnAiMove);
        }

        private void OnAiMove(FileRank from, FileRank to, string promo)
        {
            routineRunner.StartCoroutine(SimulateDelay(from, to, promo));
        }

        IEnumerator SimulateDelay(FileRank from, FileRank to, string promo)
        {
            float delay = AiMoveTimes.M_CPU;

            float timeElapsed = Time.time - startTime;
            delay -= timeElapsed;
            delay = Mathf.Max(0, delay);
            yield return new WaitForSecondsRealtime(delay);

            ExecOnAiMove(from, to, promo);
        }

        private void ExecOnAiMove(FileRank from, FileRank to, string promo)
        {
            FileRank fromFileRank = from;
            FileRank toFileRank = to;

            chessboardModel.opponentFromSquare = chessboardModel.squares[fromFileRank.file, fromFileRank.rank];
            chessboardModel.opponentToSquare = chessboardModel.squares[toFileRank.file, toFileRank.rank];
            chessboardModel.promoString = promo;

            ChessMoveResult moveResult = chessService.MakeMove(
                chessboardModel.opponentFromSquare.fileRank,
                chessboardModel.opponentToSquare.fileRank,
                chessboardModel.promoString, 
                false,
                chessboardModel.squares);

            chessboardModel.isPlayerInCheck = moveResult.isPlayerInCheck;
            chessboardModel.isOpponentInCheck = moveResult.isOpponentInCheck;
            chessboardModel.playerScore = chessService.GetScore(chessboardModel.playerColor);
            chessboardModel.opponentScore = chessService.GetScore(chessboardModel.opponentColor);
            chessboardModel.notation.Add(moveResult.description);
            chessboardModel.capturedSquare = moveResult.capturedSquare;
            chessboardModel.opponentMoveFlag = moveResult.moveFlag;
            chessboardModel.gameEndReason = moveResult.gameEndReason;

            // Clear out the player from square if the opponent
            // has captured the piece he had selected
            if (chessboardModel.opponentToSquare.Equals(chessboardModel.playerFromSquare))
            {
                chessboardModel.playerFromSquare = null;
            }

            // Finally add the move to the move list
            ChessMove move = new ChessMove();
            move.from = chessboardModel.opponentFromSquare.fileRank;
            move.to = chessboardModel.opponentToSquare.fileRank;
            move.promo = chessboardModel.promoString;
            chessboardModel.moveList.Add(move);

            chessboardEventSignal.Dispatch(ChessboardEvent.OPPONENT_MOVE_COMPLETE);

            Release();
        }
    }
}
