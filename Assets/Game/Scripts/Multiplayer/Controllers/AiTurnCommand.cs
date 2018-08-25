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
using strange.extensions.promise.api;
using TurboLabz.Chess;
using TurboLabz.InstantFramework;
using TurboLabz.TLUtils;

namespace TurboLabz.Multiplayer
{
    public class AiTurnCommand : Command
    {
        // Dispatch Signals
        [Inject] public BackendErrorSignal backendErrorSignal { get; set; }

        // Services
        [Inject] public IChessService chessService { get; set; }
        [Inject] public IChessAiService chessAiService { get; set; }
        [Inject] public IBackendService backendService { get; set; }

        // Models
        [Inject] public IPlayerModel playerModel { get; set; }
        [Inject] public IChessboardModel chessboardModel { get; set; }
        [Inject] public IMatchInfoModel matchInfoModel { get; set; }

        public override void Execute()
        {
            Retain();

            Chessboard chessboard = chessboardModel.activeChessboard;

            ++chessboard.aiMoveNumber;
            chessAiService.SetPosition(chessService.GetFen());

            AiMoveInputVO vo = new AiMoveInputVO();
            vo.aiColor = chessboard.opponentColor;
            vo.playerColor = chessboard.playerColor;
            vo.lastPlayerMove = chessboard.lastPlayerMove;
            vo.squares = chessboard.squares;
            vo.opponentTimer = chessboard.backendOpponentTimer;
            vo.aiMoveNumber = chessboard.aiMoveNumber;

            // Strength
            vo.cpuStrengthPct = matchInfoModel.activeMatch.botDifficulty;

            if (chessboard.overrideAiStrength == AiOverrideStrength.SMART)
            {
                vo.cpuStrengthPct = 1f;
            }
            else if (chessboard.overrideAiStrength == AiOverrideStrength.STUPID)
            {
                vo.cpuStrengthPct = 0f;
            }

            // Speed
            if (vo.opponentTimer.TotalSeconds < 10)
            {
                vo.aiMoveDelay = AiMoveDelay.NONE;
            }
            else if (vo.opponentTimer.TotalSeconds < 30)
            {
                vo.aiMoveDelay = AiMoveDelay.CPU;
            }
            else
            {
                vo.aiMoveDelay = AiMoveDelay.ONLINE_5M;
            }

                
            // TODO: In the future, if we add 1 minute games, use the IsOneMinuteGame flag in the vo
            // to make the bots more aggressive so people can't spam the time control.

            IPromise<FileRank, FileRank, string> promise = chessAiService.GetAiMove(vo);
            promise.Then(OnAiMove);
        }

        private void OnAiMove(FileRank from, FileRank to, string promo)
        {
            Chessboard chessboard = chessboardModel.activeChessboard;

            if (chessboard.aiWillResign &&
                chessService.GetScore(chessboard.playerColor) > BotSettings.AI_RESIGN_SCORE_THRESHOLD)
            {
                backendService.AiResign().Then(OnResign);
            }
            else
            {
                backendService.AiTurn(from, to, promo).Then(OnTurnTaken); ;
            }
        }

        private void OnTurnTaken(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }

        private void OnResign(BackendResult result)
        {
            if (result != BackendResult.SUCCESS && result != BackendResult.CANCELED)
            {
                backendErrorSignal.Dispatch(result);
            }

            Release();
        }

            
    }


}
